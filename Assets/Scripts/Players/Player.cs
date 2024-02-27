using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class Player : IOtherPlayer
{
    [SerializeField]
    protected PlayerData _data;
    [SerializeField]
    protected int _placeableTroops;
    protected IGameStatePlayerView _gameState;
    [SerializeField]
    protected bool _isMyTurn;
    protected List<TerritoryCard> _hand;
    protected bool _cardEligible;

    public Player(GameState gameState, PlayerData data, bool is2PlayerGame)
    {
        _data = data;
        _gameState = gameState;
        _isMyTurn = false;
        _hand = new List<TerritoryCard>();
        _cardEligible = false;
        if(is2PlayerGame)
        {
            _placeableTroops = 26;
        }
        else if(_gameState.Players().Length >= 3 && _gameState.Players().Length <= 6)
        {
            _placeableTroops = 40 - ((_gameState.Players().Length - 2) * 5);
        }
        else
        {
            Debug.Log("Error in player count: there are " + _gameState.Players().Length + " players, should be between 2 and 6");
            return;
        }
    }
    public Player(Player oldPlayer, GameState newState)
    {
        _data = oldPlayer._data;
        _placeableTroops = oldPlayer._placeableTroops;
        _gameState = newState;
        _isMyTurn = oldPlayer._isMyTurn;
        _hand = new List<TerritoryCard>();
        foreach(var card in oldPlayer._hand)
        {
            _hand.Add(card);
        }
        _cardEligible = oldPlayer._cardEligible;

    }

    public abstract void StartTurn();
    public abstract void AddCardsToHand(List<TerritoryCard> cards);

    public void EndTurnStage()
    {
        new EndTurnStage(this).execute();
    }

    public void SetCardEligible(bool set)
    {
        _cardEligible = set;
    }
    public bool IsCardEligible()
    {
        return _cardEligible;
    }
    public List<TerritoryCard> GetCardHand()
    {
        return _hand;
    }
    public bool IsMyTurn()
    {
        return _isMyTurn;
    }
    public int GetPlaceableTroopNumber()
    {
        return _placeableTroops;
    }
    public void SetPlaceableTroopNumber(int newNumber)
    {
        _placeableTroops = newNumber;
    }
    public void DecrementPlaceableTroops(int number)
    {
        _placeableTroops -= number;
    }

    public PlayerData GetData()
    {
        return _data;
    }
    // Function called by other objects when a player's turn should end (eg. they make a non-attacking troop move)
    public void EndTurn()
    {
        Debug.Log(_data.playerName + " ending turn");
        _isMyTurn = false;
        new EndTurn(this).execute();
    }
}