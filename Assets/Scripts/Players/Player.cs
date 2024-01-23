using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class Player : IOtherPlayer
{
    [SerializeField]
    protected PlayerData _data;
    [SerializeField]
    protected int _placeableTroops;
    [SerializeField]
    protected HashSet<ITerritoryPlayerView> _ownedTerritories;
    protected IGameMasterPlayerView _gameMaster;
    [SerializeField]
    protected bool _isMyTurn;
    protected List<TerritoryCard> _hand;
    protected bool _cardEligible;

    public Player(GameMaster gameMaster, PlayerData data)
    {
        _data = data;
        _ownedTerritories = new HashSet<ITerritoryPlayerView>();
        _gameMaster = gameMaster;
        _isMyTurn = false;
        _hand = new List<TerritoryCard>();
        _cardEligible = false;
        if(_gameMaster.is2PlayerGame)
        {
            _placeableTroops = 26;
        }
        else if(_gameMaster.Players().Length >= 3 && _gameMaster.Players().Length <= 6)
        {
            _placeableTroops = 40 - ((_gameMaster.Players().Length - 2) * 5);
        }
        else
        {
            Debug.Log("Error in player count: there are " + _gameMaster.Players().Length + " players, should be between 2 and 6");
            return;
        }
    }

    public abstract void StartTurn();
    public abstract void AddCardsToHand(List<TerritoryCard> cards);

    public void EndTurnStage()
    {
        new EndTurnStage(this, _gameMaster).execute();
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
        new EndTurn(this, _gameMaster).execute();
    }
}