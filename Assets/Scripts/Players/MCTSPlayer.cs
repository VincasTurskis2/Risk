using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditorInternal;
using System;

// An implementation of a player using Monte Carlo tree search.
public class MCTSPlayer : Player
{
    public float timeForSearch = 1f;
    public double C = Math.Sqrt(2);
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
        new UpdatePlaceableTroops(this).execute();
        DeployTroops();
        PerformMTCS();
        Fortify();

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
        GameStateTreeNode root = new GameStateTreeNode(_gameState, null);
        float curTime = 0f;
        while (curTime < timeForSearch)
        {
        //     State leaf = select(tree)
        //     State child = expand(leaf)
        //     tree.addNode(child, leaf)
        //     result = simulate(child)
        //     back_propagate(result, child)
        }
        
        
        //return {Action that has the highest number of playouts}
        return result;
    }

    public GameStateTreeNode Select(GameStateTreeNode curNode)
    {
        if(curNode.numberOfPlaythoughs == 0) return curNode;

        return curNode;
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
