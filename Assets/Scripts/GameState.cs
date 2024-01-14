using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public CardDeck cardDeck {get; private set;}
    public Player[] Players {get; set;}
    public Territory[] territories {get; set;}
    public int currentPlayerNo{get; set;}
    public TurnStage turnStage {get; set;}
    public int cardSetRewardStage {get; set;} = 0;
    public GameState(CardDeck CardDeck)
    {
        cardDeck = CardDeck;
    }
}
