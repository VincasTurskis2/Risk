using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : IGameStatePlayerView
{
    public CardDeck cardDeck {get; private set;}
    public Player[] players {get; set;}
    public Map map {get; set;}
    public int currentPlayerNo{get; set;}
    public TurnStage turnStage {get; set;}
    public int cardSetRewardStage {get; set;} = 0;
    public bool terminalState = false;
    public GameState()
    {
        cardDeck = new CardDeck();
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
            }
        }
        map = new Map(oldState.map);
        currentPlayerNo = oldState.currentPlayerNo;
        turnStage = oldState.turnStage;
        cardSetRewardStage = oldState.cardSetRewardStage;
        terminalState = oldState.terminalState;
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
}
