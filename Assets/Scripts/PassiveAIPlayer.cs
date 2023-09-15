using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// An "AI" player class which does nothing. On turn start, immediately ends its turn.
public class PassiveAIPlayer : MonoBehaviour, Player
{
    [SerializeField]
    private PlayerData _data;
    private HashSet<Territory> _ownedTerritories;
    private GameState _gameState;
    [SerializeField]
    private bool _isMyTurn = false;
    [SerializeField]
    private int _placeableTroops;
    private List<TerritoryCard> _hand;
    private PlayerActions _actions;
    private bool _cardEligible;
    public void Setup(GameState state, PlayerData data)
    {
        _gameState = state;
        if(_gameState.Players.Length >= 2 && _gameState.Players.Length <= 6)
        {
            _placeableTroops = 40 - ((_gameState.Players.Length - 2) * 5);
        }
        else
        {
            Debug.Log("Error in player count: there are " + _gameState.Players.Length + " players, should be between 2 and 6");
            return;
        }
        _actions = (PlayerActions) FindAnyObjectByType(typeof(PlayerActions));
        _ownedTerritories = new HashSet<Territory>();
        _hand = new List<TerritoryCard>();
        _data = data;
    }
    public void StartTurn()
    {
        Debug.Log(_data.playerName + " starting turn");
        _isMyTurn = true;
        _placeableTroops = _actions.CalculatePlaceableTroops(this);
        if(_gameState.turnStage == TurnStage.Setup)
        {
            List<Territory> possibleTerritories = new List<Territory>(_ownedTerritories);
            if(!_gameState.allTerritoriesClaimed)
            {
                possibleTerritories = new List<Territory>();
                foreach(Territory t in _gameState.territories)
                {
                    
                    if(t.Owner == null)
                    {
                        possibleTerritories.Add(t);
                    }
                }
            }
            int randomTerritoryNumber = Random.Range(0, possibleTerritories.Count);
            bool success = _actions.SetupDeploy(possibleTerritories[randomTerritoryNumber], this);
        }
        else
        {
            EndTurn();
        }
    }
    public void EndTurn()
    {
        Debug.Log(_data.playerName + " ending turn");
        _isMyTurn = false;
        _actions.EndTurn(this);
    }
    public void EndTurnStage()
    {
        EndTurn();
    }
    public bool IsMyTurn()
    {
        return _isMyTurn;
    }
    public HashSet<Territory> GetOwnedTerritories()
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
    public void DecrementPlaceableTroops()
    {
        _placeableTroops--;
    }
    public PlayerData GetData()
    {
        return _data;
    }
    public void AddCardsToHand(List<TerritoryCard> cards)
    {
        _hand = Enumerable.Concat(_hand, cards).ToList();
    }
    public void DiscardCards(TerritoryCard[] cardsToDiscard)
    {
        for(int i = 0; i < cardsToDiscard.Length; i++)
        {
            if(_hand.Contains(cardsToDiscard[i]))
            {
                _gameState.cardDeck.DiscardCard(cardsToDiscard[i]);
                _hand.Remove(cardsToDiscard[i]);
            }
        }
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
}
