using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// a class representing neutral territories that appear in a 2 player game
public class NeutralArmyPlayer : Player
{
    
    public NeutralArmyPlayer(GameMaster state) : base(state, null)
    {
        _data = new PlayerData("Neutral", PlayerType.Neutral, ColorPreset.White);
        _placeableTroops = 0;
    }
    public override void StartTurn()
    {
        EndTurn();
    }

    public override void AddCardsToHand(List<TerritoryCard> cards)
    {
        new DiscardCards(this, (GameMaster) _gameMaster, cards.ToArray()).execute();
    }
}