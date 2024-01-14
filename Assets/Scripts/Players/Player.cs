using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class Player : MonoBehaviour, IOtherPlayer
{
    [SerializeField]
    protected PlayerData _data;
    [SerializeField]
    protected int _placeableTroops;
    [SerializeField]
    protected HashSet<ITerritoryPlayerView> _ownedTerritories;
    protected IGameStatePlayerView _gameState;
    [SerializeField]
    protected bool _isMyTurn = false;
    protected List<TerritoryCard> _hand;
    protected PlayerActions _actions;
    protected bool _cardEligible;

    public abstract void StartTurn();
    public abstract void Setup(GameMaster state, PlayerData data);
    public abstract void DiscardCards(TerritoryCard[] cardsToDiscard);
    public abstract void AddCardsToHand(List<TerritoryCard> cards);

    public void EndTurnStage()
    {
        _actions.EndTurnStage(this);
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
    public HashSet<ITerritoryPlayerView> GetOwnedTerritories()
    {
        return _ownedTerritories;
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
        _actions.EndTurn(this);
    }
}