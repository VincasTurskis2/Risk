using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// a class representing neutral territories that appear in a 2 player game
public class NeutralArmyPlayer : Player
{
    
    public override void Setup(GameMaster state, PlayerData data)
    {
        _gameState = state;
        if(_gameState.Players().Length == 3)
        {
            _placeableTroops = 26;
        }
        else
        {
            Debug.Log("Error: Neutral player should not appear in non-2 player games");
            return;
        }
        _actions = (PlayerActions) FindAnyObjectByType(typeof(PlayerActions));
        _ownedTerritories = new HashSet<ITerritoryPlayerView>();
        _hand = new List<TerritoryCard>();
        _data = new PlayerData("Neutral", PlayerType.Neutral, ColorPreset.White);
    }
    public override void StartTurn()
    {
        EndTurn();
    }

    public override void AddCardsToHand(List<TerritoryCard> cards)
    {
        new DiscardCards(this, (GameMaster) _gameState, cards.ToArray()).execute();
    }
}