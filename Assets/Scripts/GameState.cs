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
    public GameState()
    {
        cardDeck = new CardDeck();
    }

    public GameState(GameState oldState)
    {
        cardDeck = new CardDeck(oldState.cardDeck);
        
        map = new Map(oldState.map);
        currentPlayerNo = oldState.currentPlayerNo;
        turnStage = oldState.turnStage;
        cardSetRewardStage = oldState.cardSetRewardStage;
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
