using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameState : IGameStatePlayerView
{
    public CardDeck cardDeck {get; private set;}
    public Player[] players {get; set;}
    public Map map {get; set;}
    public int currentPlayerNo{get; set;}
    public TurnStage turnStage {get; set;}
    public int cardSetRewardStage {get; set;} = 0;
    public bool terminalState = false;
    public bool simulationState = false;
    public bool handlePlayerLoss = false;
    public GameState()
    {
        cardDeck = new CardDeck();
        terminalState = false;
        simulationState = false;
        handlePlayerLoss = false;

    }

    public GameState(GameState oldState)
    {
        cardDeck = new CardDeck(oldState.cardDeck);
        players = new Player[oldState.players.Length];
        for(int i = 0; i < oldState.players.Length; i++)
        {
            switch (oldState.players[i].GetData().playerType)
            {
                case PlayerType.Human:
                    players[i] = new HumanPlayer((HumanPlayer)oldState.players[i], this);
                    break;
                case PlayerType.PassiveAI:
                    players[i] = new PassiveAIPlayer((PassiveAIPlayer)oldState.players[i], this);
                    break;
                case PlayerType.Neutral:
                    players[i] = new NeutralArmyPlayer((NeutralArmyPlayer)oldState.players[i], this);
                    break;
                case PlayerType.MCTS:
                    players[i] = new MCTSPlayer((MCTSPlayer)oldState.players[i], this);
                    break;
                case PlayerType.MCTSBenchmark:
                    players[i] = new MCTSBenchmarkPlayer((MCTSBenchmarkPlayer)oldState.players[i], this);
                    break;
            }
        }
        map = new Map(oldState.map);
        currentPlayerNo = oldState.currentPlayerNo;
        turnStage = oldState.turnStage;
        cardSetRewardStage = oldState.cardSetRewardStage;
        terminalState = oldState.terminalState;
        simulationState = oldState.simulationState;
    }

    public IOtherPlayer[] Players()
    {
        return players;
    }

    public IMapPlayerView Map()
    {
        return map;
    }
    
    public Player getPlayerFromName(string playerName)
    {
        for(int i = 0; i < players.Length; i++)
        {
            if(playerName.Equals(players[i].GetData().playerName))
            {
                return players[i];
            }
        }
        return null;
    }
    public IOtherPlayer getPlayerViewFromName(string playerName)
    {
        return getPlayerFromName(playerName);
    }

    public List<Attack> getAllPossibleAttacks(Player player)
    {
        List<Attack> result = new();
        ITerritoryPlayerView[] curPlayerTerritories = map.GetOwnedTerritories(player);
        for(int i = 0; i < curPlayerTerritories.Length; i++)
        {
            TerritoryData curTerritory = (TerritoryData) curPlayerTerritories[i];
            if(curTerritory.TroopCount == 1) continue;
            TerritoryData[] unownedNeighbors = map.GetRawTerritories(curTerritory.Neighbors).Where(x => x.Owner.Equals(player.GetData().playerName) == false).ToArray();
            foreach(var neighbor in unownedNeighbors)
            {
                result.Add(new Attack(getPlayerFromName(curTerritory.Owner), curTerritory, neighbor));
            }
        }
        return result;
    }
    public List<Attack> getAllPossibleAttacks()
    {
        return getAllPossibleAttacks(players[currentPlayerNo]);
    }
}
