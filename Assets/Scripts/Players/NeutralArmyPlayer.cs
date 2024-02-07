using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// a class representing neutral territories that appear in a 2 player game
public class NeutralArmyPlayer : Player
{
    
    public NeutralArmyPlayer(GameState state) : base(state, null, true)
    {
        _data = new PlayerData("Neutral", PlayerType.Neutral, ColorPreset.White);
        _placeableTroops = 0;
    }
    public NeutralArmyPlayer(NeutralArmyPlayer oldPlayer, GameState newState) : base(oldPlayer, newState)
    {
    }
    public override void StartTurn()
    {
        EndTurn();
    }

    public override void AddCardsToHand(List<TerritoryCard> cards)
    {
        new DiscardCards(this, cards.ToArray()).execute();
    }
}