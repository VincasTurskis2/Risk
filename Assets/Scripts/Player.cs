using UnityEngine;
using System.Collections.Generic;

public abstract class Player : MonoBehaviour
{
    public abstract void EndTurn();
    public abstract void StartTurn();
    public abstract void EndTurnStage();
    public abstract bool IsMyTurn();
    public abstract HashSet<Territory> GetOwnedTerritories();
    public abstract int GetPlaceableTroopNumber();
    public abstract void SetPlaceableTroopNumber(int newNumber);
    public abstract void DecrementPlaceableTroops();
    public abstract void Setup(GameState state, PlayerData data);
    public abstract List<TerritoryCard> GetCardHand();
    public abstract void AddCardsToHand(List<TerritoryCard> cards);
    public abstract void DiscardCards(TerritoryCard[] cardsToDiscard);
    public abstract PlayerData GetData();

    public abstract void SetCardEligible(bool set);
    public abstract bool IsCardEligible();
}