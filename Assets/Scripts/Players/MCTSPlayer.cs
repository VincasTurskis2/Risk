using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// An implementation of Monte Carlo tree search.
public class MCTSPlayer : Player
{
    
    public override void Setup(GameState state, PlayerData data)
    {
        _gameState = state;
        if(_gameState.is2PlayerGame)
        {
            _placeableTroops = 26;
        }
        else if(_gameState.Players.Length >= 3 && _gameState.Players.Length <= 6)
        {
            _placeableTroops = 40 - ((_gameState.Players.Length - 2) * 5);
        }
        else
        {
            Debug.Log("Error in player count: there are " + _gameState.Players.Length + " players, should be between 2 and 6");
            return;
        }
        _actions = (PlayerActions) FindAnyObjectByType(typeof(PlayerActions));
        _ownedTerritories = new HashSet<ITerritoryPlayerView>();
        _hand = new List<TerritoryCard>();
        _data = data;
    }
    public override void StartTurn()
    {
        Debug.Log(_data.playerName + " starting turn");
        _isMyTurn = true;
        _placeableTroops = _actions.CalculatePlaceableTroops(this);
    }
    
    public override void DiscardCards(TerritoryCard[] cardsToDiscard)
    {
        _actions.DiscardCards(cardsToDiscard, this);
    }
    public override void AddCardsToHand(List<TerritoryCard> cards)
    {
        if(cards == null) return;
        _hand = Enumerable.Concat(_hand, cards).ToList();
    }
}
