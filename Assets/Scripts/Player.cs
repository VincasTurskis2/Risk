using UnityEngine;
using System.Collections.Generic;

public interface Player
{
    public void EndTurn();
    public void StartTurn();
    public void EndTurnStage();
    public bool IsMyTurn();
    public HashSet<Territory> GetOwnedTerritories();
    public int GetPlaceableTroopNumber();
    public void SetPlaceableTroopNumber(int newNumber);
    public void DecrementPlaceableTroops();
    public void Setup(GameState state, PlayerData data);
    public List<TerritoryCard> GetCardHand();
    public void AddCardsToHand(List<TerritoryCard> cards);
    public void DiscardCards(TerritoryCard[] cardsToDiscard);
    public PlayerData GetData();

    public void SetCardEligible(bool set);
    public bool IsCardEligible();
}