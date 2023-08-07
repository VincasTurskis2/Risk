using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private int _cardSetRewardStage = 0;
    private List<TerritoryCard> _hand;
    public void Setup(GameState state, PlayerData data)
    {
        _placeableTroops = 0;
        _gameState = state;
        _ownedTerritories = new HashSet<Territory>();
        _hand = new List<TerritoryCard>();
        _data = data;
    }
    public void StartTurn()
    {
         Debug.Log(_data.playerName + " starting turn");
        EndTurn();
    }
    public void EndTurn()
    {
         Debug.Log(_data.playerName + " ending turn");
        _gameState.EndTurn();
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
    public void IncrementCardSetReward()
    {
        _cardSetRewardStage++;
    }

    public int GetCardSetRewardStage()
    {
        return _cardSetRewardStage;
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
}
