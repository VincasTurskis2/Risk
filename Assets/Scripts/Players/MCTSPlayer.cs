using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEditorInternal;

// An implementation of a player using Monte Carlo tree search.
public class MCTSPlayer : Player
{
    public float timeForSearch = 1f;
    //C = 0.5 because Limer et al. suggests so.
    public double C = 0.5;
    public int depth = 8;
    public MCTSPlayer(GameState state, PlayerData data, bool is2PlayerGame) : base(state, data, is2PlayerGame)
    {
    }
    public MCTSPlayer(MCTSPlayer oldPlayer, GameState newState) : base(oldPlayer, newState)
    {
    }
    public override void StartTurn()
    {
        Debug.Log(_data.playerName + " starting turn");
        _isMyTurn = true;

        if(_gameState.turnStage == TurnStage.InitDeploy || _gameState.turnStage == TurnStage.InitReinforce)
        {
            List<ITerritoryPlayerView> possibleTerritories = _gameState.Map().GetOwnedTerritories(this).ToList();
            if(_gameState.turnStage == TurnStage.InitDeploy)
            {
                possibleTerritories = new List<ITerritoryPlayerView>();
                foreach(ITerritoryPlayerView t in _gameState.Map().GetTerritories())
                {
                    if(t.GetOwner() == null)
                    {
                        possibleTerritories.Add(t);
                    }
                }
            }
            int randomTerritoryNumber = UnityEngine.Random.Range(0, possibleTerritories.Count);
            bool success = new SetupDeploy(this, possibleTerritories[randomTerritoryNumber]).execute();
        }
        new UpdatePlaceableTroops(this).execute();
        DeployTroops();
        PerformMTCS();
        Fortify();
        _isMyTurn = false;
    }
    
    public override void AddCardsToHand(List<TerritoryCard> cards)
    {
        if(cards == null) return;
        _hand = Enumerable.Concat(_hand, cards).ToList();
    }

    // A method to make a rule-based selection on where to deploy troops
    private void DeployTroops()
    {
        ITerritoryPlayerView[] myTerritories = _gameState.Map().GetOwnedTerritories(this);
        while(_placeableTroops > 0)
        {
            float maxRatio = 0;
            ITerritoryPlayerView toDeploy = myTerritories[UnityEngine.Random.Range(0, myTerritories.Length)];
            for(int i = 0; i < myTerritories.Length; i++)
            {
                float ratio = TroopRatio(myTerritories[i]);
                if(ratio > maxRatio)
                {
                    maxRatio = ratio;
                    toDeploy = myTerritories[i];
                }
            }
            new Deploy(this, toDeploy).execute();
        }
        if(_gameState.turnStage == TurnStage.Deploy)
        {
            Debug.Log("MCTS Player: Manually ending turn stage");
            new EndTurnStage(this).execute();
        }
    }
    private int NeighboringTroops(ITerritoryPlayerView territory)
    {
        int neighborTroopCount = 0;
        ITerritoryPlayerView[] neighbors = _gameState.Map().GetTerritories(territory.GetNeighbors());
        foreach(var neighbor in neighbors)
        {
            if(!neighbor.GetOwner().Equals(_data.playerName))
            {
                neighborTroopCount += neighbor.TroopCount;
            }
        }
        return neighborTroopCount;
    }
    private float TroopRatio(ITerritoryPlayerView territory)
    {
        int neighborTroopCount = NeighboringTroops(territory);
        float ratio = neighborTroopCount/territory.TroopCount;
        return ratio;
    }

    private void Fortify()
    {
        ITerritoryPlayerView[] myTerritories = _gameState.Map().GetOwnedTerritories(this);
        ITerritoryPlayerView strongestInlandTerritory = null;
        foreach(var territory in myTerritories)
        {
            if(territory.TroopCount > 1 && NeighboringTroops(territory) == 0)
            {
                if(strongestInlandTerritory == null)
                {
                    strongestInlandTerritory = territory;
                }
                else if(strongestInlandTerritory.TroopCount < territory.TroopCount)
                {
                    strongestInlandTerritory = territory;
                }
            }
        }
        if(strongestInlandTerritory == null)
        {
            new EndTurnStage(this).execute();
            return;
        }
        ITerritoryPlayerView[] neighbors = _gameState.Map().GetTerritories(strongestInlandTerritory.GetNeighbors());
        float maxRatio = 0;
        ITerritoryPlayerView toFortify = null;
        foreach(var neighbor in neighbors)
        {
            float ratio = TroopRatio(neighbor);
            if(ratio > maxRatio)
            {
                maxRatio = ratio;
                toFortify = neighbor;
            }
        }
        if(toFortify == null)
        {
            new EndTurnStage(this).execute();
            return;
        }
        new Fortify(this, strongestInlandTerritory, toFortify, strongestInlandTerritory.TroopCount - 1).execute();
    }



    public List<PlayerAction> PerformMTCS()
    {
        List<PlayerAction> result = new();
        GameStateTreeNode root = new GameStateTreeNode(_gameState, null, null);
        float curTime = 0f;
        while (curTime < timeForSearch)
        {
            GameStateTreeNode nodeToExplore = Select(root);
            var expandedNode = nodeToExplore.Expand();
            float newResult = Simulate(expandedNode);
            Backpropagate(expandedNode, newResult);
        }
        return result;
    }
    public GameStateTreeNode Select(GameStateTreeNode node)
    {
        while(node.fullyExpanded == true)
        {
            float[] ucb1Values = new float[node.children.Count()];
            int maxIndex = -1;
            float maxValue = -1;
            for(int i = 0; i < node.children.Count(); i++)
            {
                var child = node.children[i];
                ucb1Values[i] = UCB1(child);
                if(ucb1Values[i] > maxValue)
                {
                    maxValue = ucb1Values[i];
                    maxIndex = i;
                }
            }
            node = node.children[maxIndex];
        }
        return node;
    }

    public float Simulate(GameStateTreeNode node)
    {
        GameState originalState = (GameState)_gameState;
        GameState curState = new GameState(node.state);
        GameMaster.Instance.state = new GameState(curState);
        int curDepth = 0;
        Player toSimulate = curState.players[curState.currentPlayerNo];
        SimulationHelper.SimulationAttack(curState.players[curState.currentPlayerNo], curState);
        SimulationHelper.SimulationReinforce(curState.players[curState.currentPlayerNo], curState);
        curState.currentPlayerNo += 1;
        if(curState.currentPlayerNo >= curState.players.Length)
        {
            curState.currentPlayerNo = 0;
        }
        while(curDepth < depth || curState.map.GetOwnedTerritories(toSimulate).Length == 0)
        {
            for(int i = curState.currentPlayerNo; curState.players[i] != toSimulate; i++)
            {
                if(curState.players[i].GetType() != typeof(NeutralArmyPlayer))
                {
                    SimulationHelper.SimulationDeploy(curState.players[curState.currentPlayerNo], curState);
                    SimulationHelper.SimulationAttack(curState.players[curState.currentPlayerNo], curState);
                    SimulationHelper.SimulationReinforce(curState.players[curState.currentPlayerNo], curState);
                }
                curState.currentPlayerNo += 1;
                if(curState.currentPlayerNo >= curState.players.Length)
                {
                    curState.currentPlayerNo = 0;
                }
            }
            curDepth += 1;
        }
        float result = heuristicEvaluation(curState, toSimulate);
        GameMaster.Instance.state = originalState;
        return result;
    }

    public void Backpropagate(GameStateTreeNode node, float newResult)
    {
        while(node.parent != null)
        {
            if(node.state.players[node.state.currentPlayerNo].GetData().playerName.Equals(node.parent.state.players[node.parent.state.currentPlayerNo].GetData().playerName))
            {
                newResult = 0 - newResult;
            }
            node.cumulativeHeuristicValue += newResult;
            node.numberOfPlaythoughs++;
            node = node.parent;
        }
        node.cumulativeHeuristicValue += newResult;
        node.numberOfPlaythoughs++;
    }

    public float heuristicEvaluation(GameState state, Player player)
    {
        return troopSharePercent(state, player);
    }

    public float troopSharePercent(GameState state, Player player)
    {
        int totalOwnTroops = 0, totalOtherTroops = 0;
        for(int i = 0; i < state.map.Territories.Length; i++)
        {
            if(state.map.Territories[i].Owner.Equals(player.GetData().playerName))
            {
                totalOwnTroops += state.map.Territories[i].TroopCount;
            }
            else
            {
                totalOtherTroops += state.map.Territories[i].TroopCount;
            }
        };
        if(totalOwnTroops + totalOtherTroops == 0) return 0;
        
        return totalOwnTroops/(totalOwnTroops + totalOtherTroops);
        
    }
    public float UCB1(GameStateTreeNode node)
    {
        double result = 0;
        if(node.numberOfPlaythoughs != 0)
        {
            result = node.cumulativeHeuristicValue / node.numberOfPlaythoughs;
        }
        if(node.parent != null)
        {
            result += C * Math.Sqrt(Math.Log(node.parent.numberOfPlaythoughs)/node.numberOfPlaythoughs);
        }
        return (float)result;
    }
}
