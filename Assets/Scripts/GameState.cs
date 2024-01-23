using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public CardDeck cardDeck {get; private set;}
    public Player[] Players {get; set;}
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
        cardSetRewardStage = oldState.cardSetRewardStage;
        currentPlayerNo = oldState.currentPlayerNo;
        turnStage = oldState.turnStage;
        //map = oldState.map;
    }
}
