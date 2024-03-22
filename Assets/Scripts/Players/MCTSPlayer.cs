using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Threading;

// An implementation of a player using Monte Carlo tree search.
public class MCTSPlayer : Player
{
    public float timeForSearch = 1f;
    public int maxNumOfIterations = 4000;
    //Limer et al suggest c=0.5, but picked 0.4 based on testing
    public double C = 0.4;
    public int depth = 7;
    public MonoBehaviour mono = GameMaster.Instance; // This should allow me to run MCTS as coroutine.
    public MCTSPlayer(GameState state, PlayerData data, bool is2PlayerGame) : base(state, data, is2PlayerGame)
    {
    }
    public MCTSPlayer(MCTSPlayer oldPlayer, GameState newState) : base(oldPlayer, newState)
    {
    }
    public override void StartTurn()
    {
        if (!GameMaster.Instance.isAIOnlyGame)
        {
            Debug.Log(_data.playerName + " starting turn");
        }
        _isMyTurn = true;

        new UpdatePlaceableTroops(this).execute();
        if (_gameState.turnStage == TurnStage.InitDeploy || _gameState.turnStage == TurnStage.InitReinforce)
        {
            Strategies.InitDeploy_StrongestContinent((GameState)_gameState, this);
        }
        else
        {
            new TradeInAnyCards(this).execute();
            DeployTroops();
            mono.StartCoroutine(RunMCTS());
        }
    }

    public override void AddCardsToHand(List<TerritoryCard> cards)
    {
        if (cards == null) return;
        _hand = Enumerable.Concat(_hand, cards).ToList();
    }

    // A method to make a rule-based selection on where to deploy troops
    private void DeployTroops()
    {
        Strategies.Deploy_FavouriteContinent((GameState)_gameState, this);
    }
    private int NeighboringTroops(ITerritoryPlayerView territory, GameState gameState, Player player)
    {
        int neighborTroopCount = 0;
        ITerritoryPlayerView[] neighbors = gameState.Map().GetTerritories(territory.GetNeighbors());
        foreach (var neighbor in neighbors)
        {
            if (!neighbor.GetOwner().Equals(player.GetData().playerName))
            {
                neighborTroopCount += neighbor.TroopCount;
            }
        }
        return neighborTroopCount;
    }
    private float TroopRatio(ITerritoryPlayerView territory, GameState gameState, Player player)
    {
        int neighborTroopCount = NeighboringTroops(territory, gameState, player);
        float ratio = neighborTroopCount / territory.TroopCount;
        return ratio;
    }

    private void Fortify()
    {
        ITerritoryPlayerView[] myTerritories = _gameState.Map().GetOwnedTerritories(this);
        ITerritoryPlayerView strongestInlandTerritory = null;
        foreach (var territory in myTerritories)
        {
            if (territory.TroopCount > 1 && NeighboringTroops(territory, (GameState)_gameState, this) == 0)
            {
                if (strongestInlandTerritory == null)
                {
                    strongestInlandTerritory = territory;
                }
                else if (strongestInlandTerritory.TroopCount < territory.TroopCount)
                {
                    strongestInlandTerritory = territory;
                }
            }
        }
        if (strongestInlandTerritory == null)
        {
            new EndTurnStage(this).execute();
            return;
        }
        ITerritoryPlayerView[] neighbors = _gameState.Map().GetTerritories(strongestInlandTerritory.GetNeighbors());
        float maxRatio = 0;
        ITerritoryPlayerView toFortify = null;
        foreach (var neighbor in neighbors)
        {
            float ratio = TroopRatio(neighbor, (GameState)_gameState, this);
            if (ratio > maxRatio)
            {
                maxRatio = ratio;
                toFortify = neighbor;
            }
        }
        if (toFortify == null)
        {
            new EndTurnStage(this).execute();
            return;
        }
        new Fortify(this, strongestInlandTerritory, toFortify, strongestInlandTerritory.TroopCount - 1).execute();
        if (_gameState.turnStage == TurnStage.Reinforce)
        {
            new EndTurnStage(this).execute();
        }
    }



    public IEnumerator RunMCTS()
    {
        List<Attack> result = new();
        var rootState = new GameState((GameState)_gameState);
        rootState.simulationState = true;
        GameStateTreeNode root = new GameStateTreeNode(rootState, null, null);
        int curIterationNo = 0;
        float startTime = GameMaster.Instance.gameTimeElapsedSeconds;
        while (curIterationNo < maxNumOfIterations)//(curTime < timeForSearch)
        {
            GameStateTreeNode nodeToExplore = Select(root);
            var expandedNode = nodeToExplore.Expand(nodeToExplore.state.players[nodeToExplore.state.currentPlayerNo]);
            float newResult = Simulate(expandedNode);
            Backpropagate(expandedNode, newResult);
            curIterationNo++;
            Debug.Log("Completed MCTS iteration");
            yield return null;
        }
        float timeDiff = GameMaster.Instance.gameTimeElapsedSeconds - startTime;
        _isMyTurn = true; // Concerning that I need this, but there's a bug somewhere and I don't have the time.
        var curNode = root;
        while (curNode.sourceMove != null || curNode.parent == null)
        {
            int maxPlaythrough = -1;

            GameStateTreeNode maxNode = null;
            foreach (var child in curNode.children)
            {
                if (child.numberOfPlaythoughs > maxPlaythrough)
                {
                    maxPlaythrough = child.numberOfPlaythoughs;
                    maxNode = child;
                }
            }
            if(maxNode == null)
            {
                result.Add(null);
                break;
            }
            result.Add(maxNode.sourceMove);
            curNode = maxNode;
        }
        if(GameMaster.Instance.isAIOnlyGame)
        {
            Logger.Instance.RecordTurnData(GetData().playerName, timeDiff, result.Count - 1);
        }
        for (int i = 0; i < result.Count(); i++)
        {
            var curMove = result[i];
            if (curMove == null)
            {
                if(GameMaster.Instance.state.turnStage == TurnStage.Attack)
                {
                    new EndTurnStage(this).execute();
                }
                Fortify();
                _isMyTurn = false;
                yield break;
            }
            bool success = true;
            var from = _gameState.Map().GetTerritory(curMove.IFrom.TerritoryName);
            var to = _gameState.Map().GetTerritory(curMove.ITo.TerritoryName);
            if (!GameMaster.Instance.isAIOnlyGame)
            {
                Debug.Log(GetData().playerName + ": Attacking from " + from.TerritoryName + " to " + to.TerritoryName);
            }
            while (success)
            {
                var attack = new Attack(this, from, to);
                success = attack.execute();
                if(GameMaster.Instance.state.terminalState == true)
                {
                    _isMyTurn = false;
                    yield break;
                }
            }
            if(from.GetOwner().Equals(to.GetOwner()))
            {
                new Occupy(this, from, to, from.TroopCount - 1).execute();
            }
        }
        if (_gameState.turnStage == TurnStage.Attack)
        {
            new EndTurnStage(this).execute();
        }
        Fortify();
        _isMyTurn = false;
    }
    public GameStateTreeNode Select(GameStateTreeNode node)
    {
        while (node.fullyExpanded == true && (node.sourceMove != null || node.parent == null) && node.state.terminalState == false)
        {
            float[] ucb1Values = new float[node.children.Count()];
            int maxIndex = -1;
            float maxValue = -1;
            for (int i = 0; i < node.children.Count(); i++)
            {
                var child = node.children[i];
                ucb1Values[i] = UCB1(child);
                if (ucb1Values[i] > maxValue)
                {
                    maxValue = ucb1Values[i];
                    maxIndex = i;
                }
            }
            if(maxIndex == -1)
            {
                return node;
            }
            node = node.children[maxIndex];
        }
        return node;
    }

    public float Simulate(GameStateTreeNode node)
    {
        if(node.state.terminalState == true)
        {
            return 1;
        }
        // Save the real active game state
        GameState originalState = GameMaster.Instance.state;
        // Create a clone that will be simulated on
        GameState curState = new GameState(node.state);

        GameMaster.Instance.state = curState;
        GameMaster.Instance.state.simulationState = true;
        int curDepth = 0;
        Player toSimulate = curState.players[curState.currentPlayerNo];
        bool cardEligible = false;
        if (node.state.turnStage == TurnStage.Attack)
        {
            cardEligible = SimulationAttack(curState.players[curState.currentPlayerNo], curState);
            if (curState.handlePlayerLoss)
                if (curState.terminalState == true)
                {
                    for (int j = 0; j < curState.players.Length; j++)
                    {
                        if (curState.players[j].GetData().playerName.Equals(GetData().playerName))
                        {
                            GameMaster.Instance.state = originalState;
                            return 1;
                        }
                    }
                    GameMaster.Instance.state = originalState;
                    return 0;
                }
            {
                curState.handlePlayerLoss = false;
            }
        }
        if (cardEligible)
        {
            List<TerritoryCard> newCard = new()
            {
                GameMaster.Instance.state.cardDeck.DrawCard()
            };
            curState.players[curState.currentPlayerNo].AddCardsToHand(newCard);
            curState.players[curState.currentPlayerNo].SetCardEligible(false);
        }
        SimulationReinforce(curState.players[curState.currentPlayerNo], curState);
        //curState.currentPlayerNo += 1;
        if (curState.currentPlayerNo >= curState.players.Length)
        {
            curState.currentPlayerNo = 0;
        }
        // Finish the current turn
        for (int i = curState.currentPlayerNo; i < curState.players.Length; i++)
        {
            string curPlayerName = curState.players[i].GetData().playerName;
            curState.players[i].SetMyTurn(true);
            if (curState.players[i].GetType() != typeof(NeutralArmyPlayer))
            {
                SimulationDeploy(curState.players[i], curState);
                cardEligible = SimulationAttack(curState.players[i], curState);
                if (curState.handlePlayerLoss)
                {
                    for (int j = 0; j < curState.players.Length; j++)
                    {
                        if (curState.players[j].GetData().playerName.Equals(curPlayerName))
                        {
                            i = j;
                            break;
                        }
                    }
                    if(curState.terminalState == true)
                    {
                        for (int j = 0; j < curState.players.Length; j++)
                        {
                            if (curState.players[j].GetData().playerName.Equals(GetData().playerName))
                            {
                                GameMaster.Instance.state = originalState;
                                return 1;
                            }
                        }
                        GameMaster.Instance.state = originalState;
                        return 0;
                    }
                    curState.handlePlayerLoss = false;
                }
                if (cardEligible)
                {
                    List<TerritoryCard> newCard = new()
                        {
                            GameMaster.Instance.state.cardDeck.DrawCard()
                        };
                    curState.players[i].AddCardsToHand(newCard);
                    curState.players[i].SetCardEligible(false);
                }
            }
            SimulationReinforce(curState.players[i], curState);
            curState.players[i].SetMyTurn(false);
            curState.players[(i + 1) % curState.players.Length].SetMyTurn(true);
            cardEligible = false;
        }
        curDepth += 1;
        while (curDepth < depth && curState.map.GetOwnedTerritories(toSimulate).Length != 0 && curState.terminalState != true)
        {
            for (int i = 0; i < curState.players.Length; i++)
            {
                string curPlayerName = curState.players[i].GetData().playerName;
                curState.currentPlayerNo = i;
                curState.players[i].SetMyTurn(true);
                if (curState.players[i].GetType() != typeof(NeutralArmyPlayer))
                {
                    SimulationDeploy(curState.players[i], curState);
                    cardEligible = SimulationAttack(curState.players[i], curState);
                    if (curState.terminalState == true)
                    {
                        break;
                    }
                    if (curState.handlePlayerLoss)
                    {
                        for(int j = 0; j < curState.players.Length; j++)
                        {
                            if (curState.players[j].GetData().playerName.Equals(curPlayerName))
                            {
                                i = j;
                                break;
                            }
                        }
                        if (curState.terminalState == true)
                        {
                            for (int j = 0; j < curState.players.Length; j++)
                            {
                                if (curState.players[j].GetData().playerName.Equals(GetData().playerName))
                                {
                                    GameMaster.Instance.state = originalState;
                                    return 1;
                                }
                            }
                            GameMaster.Instance.state = originalState;
                            return 0;
                        }
                        curState.handlePlayerLoss = false;
                    }
                    if (cardEligible)
                    {
                        List<TerritoryCard> newCard = new()
                        {
                            GameMaster.Instance.state.cardDeck.DrawCard()
                        };
                        curState.players[i].AddCardsToHand(newCard);
                        curState.players[i].SetCardEligible(false);
                    }
                }
                SimulationReinforce(curState.players[i], curState);
                curState.players[i].SetMyTurn(false);
                curState.players[(i+1)%curState.players.Length].SetMyTurn(true);
                cardEligible = false;
            }
            curDepth += 1;
        }
        float result = heuristicEvaluation(curState, toSimulate);
        GameMaster.Instance.state = originalState;
        return result;
    }

    public void Backpropagate(GameStateTreeNode node, float newResult)
    {
        while (node.parent != null)
        {
            if (!node.state.players[node.state.currentPlayerNo].GetData().playerName.Equals(node.parent.state.players[node.parent.state.currentPlayerNo].GetData().playerName))
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
        return (troopSharePercent(state, player) + troopEarningPercent(state, player)) / 2;
        //return troopSharePercent(state, player);
        //return troopEarningPercent(state, player);
        //return UnityEngine.Random.Range(0, 1);
    }

    public float troopSharePercent(GameState state, Player player)
    {
        int totalOwnTroops = 0, totalOtherTroops = 0;
        for (int i = 0; i < state.map.Territories.Length; i++)
        {
            if (state.map.Territories[i].Owner.Equals(player.GetData().playerName))
            {
                totalOwnTroops += state.map.Territories[i].TroopCount;
            }
            else
            {
                totalOtherTroops += state.map.Territories[i].TroopCount;
            }
        };
        if (totalOwnTroops + totalOtherTroops == 0) return 0;

        float result = (float)totalOwnTroops / (float)(totalOwnTroops + totalOtherTroops);
        return result;

    }
    public float troopEarningPercent(GameState state, Player player)
    {
        int ownTroopsEarned = 0;
        ownTroopsEarned += state.map.GetOwnedTerritories(player).Count() / 3;
        ownTroopsEarned += GetContinentTroopCount(state, player);
        return (float)ownTroopsEarned / 38.0f;
    }

    private int GetContinentTroopCount(GameState state, Player player)
    {
        List<Continent> ownedContinents = new List<Continent>();
        int[] continentCountLocal = new int[GameMaster.ContinentCount.Length];
        GameMaster.ContinentCount.CopyTo(continentCountLocal, 0);
        foreach (TerritoryData t in state.map.GetOwnedTerritories(player))
        {
            continentCountLocal[(int)t.Continent]--;
        }
        for (int i = 0; i < continentCountLocal.Length; i++)
        {
            if (continentCountLocal[i] <= 0)
            {
                ownedContinents.Add((Continent)i);
            }
        }
        int result = 0;
        for (int i = 0; i < ownedContinents.Count; i++)
        {
            result += GameMaster.ContinentValues[(int)ownedContinents[i]];
        }
        return result;
    }
    public float UCB1(GameStateTreeNode node)
    {
        double result = 0;
        if (node.numberOfPlaythoughs != 0)
        {
            result = node.cumulativeHeuristicValue / node.numberOfPlaythoughs;
        }
        if (node.parent != null)
        {
            result += C * Math.Sqrt(Math.Log(node.parent.numberOfPlaythoughs) / node.numberOfPlaythoughs);
        }
        return (float)result;
    }

    public bool SimulationAttack(Player player, GameState state)
    {
        return Strategies.Attack_RandomInFavouriteContinent(state, player);
    }


    public void SimulationReinforce(Player player, GameState state)
    {
        ITerritoryPlayerView[] myTerritories = state.Map().GetOwnedTerritories(player);
        ITerritoryPlayerView strongestInlandTerritory = null;
        foreach (var territory in myTerritories)
        {
            if (territory.TroopCount > 1 && NeighboringTroops(territory, state, player) == 0)
            {
                if (strongestInlandTerritory == null)
                {
                    strongestInlandTerritory = territory;
                }
                else if (strongestInlandTerritory.TroopCount < territory.TroopCount)
                {
                    strongestInlandTerritory = territory;
                }
            }
        }
        if (strongestInlandTerritory == null)
        {
            state.turnStage = TurnStage.Deploy;
            state.currentPlayerNo++;
            if (state.currentPlayerNo >= state.players.Count())
            {
                state.currentPlayerNo = 0;
            }
            return;
        }
        ITerritoryPlayerView[] neighbors = state.Map().GetTerritories(strongestInlandTerritory.GetNeighbors());
        float maxRatio = 0;
        ITerritoryPlayerView toFortify = null;
        foreach (var neighbor in neighbors)
        {
            float ratio = TroopRatio(neighbor, state, player);
            if (ratio > maxRatio)
            {
                maxRatio = ratio;
                toFortify = neighbor;
            }
        }
        if (toFortify == null)
        {
            state.turnStage = TurnStage.Deploy;
            state.currentPlayerNo++;
            if (state.currentPlayerNo >= state.players.Count())
            {
                state.currentPlayerNo = 0;
            }
            return;
        }
        new Fortify(player, strongestInlandTerritory, toFortify, strongestInlandTerritory.TroopCount - 1).execute(false);
        /*if (state.players[state.currentPlayerNo].GetData().playerName.Equals(GetData().playerName))
        {
            new EndTurnStage(this).execute();
        }*/
        state.turnStage = TurnStage.Deploy;
        state.currentPlayerNo++;
        if(state.currentPlayerNo >= state.players.Count())
        {
            state.currentPlayerNo = 0;
        }
    }

    public void SimulationDeploy(Player player, GameState state)
    {
        new UpdatePlaceableTroops(player).execute();
        new TradeInAnyCards(player).execute();
        Strategies.Deploy_FavouriteContinent(state, player);
        if (state.turnStage == TurnStage.Deploy)
        {
            state.turnStage = TurnStage.Attack;
        }
    }
}