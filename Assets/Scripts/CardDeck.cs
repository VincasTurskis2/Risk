using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// A class that manages the territory cards and their discard
public class CardDeck
{
    private Stack<TerritoryCard> _deck;
    private Stack<TerritoryCard> _discardDeck;

    public CardDeck()
    {
        
    }
    public CardDeck(CardDeck oldDeck)
    {
        _deck = new Stack<TerritoryCard>(oldDeck._deck.Count);
        _discardDeck = new Stack<TerritoryCard>(oldDeck._discardDeck.Count);
        foreach(TerritoryCard card in oldDeck._deck)
        {
            _deck.Push(new TerritoryCard(card));
        }
        foreach(TerritoryCard card in oldDeck._discardDeck)
        {
            _discardDeck.Push(new TerritoryCard(card));
        }
        Shuffle();
    }

    public void Setup(TerritoryData[] territories)
    {
        int infantryRem = 14, cavalryRem = 14, artilleryRem = 14;
        _deck = new Stack<TerritoryCard>(territories.Length + 2);
        _discardDeck = new Stack<TerritoryCard>(territories.Length + 2);
        for(int i = 0; i < territories.Length; i++)
        {
            TerritoryData t = territories[i];
            Debug.Log(t);
            int rand = Random.Range(1, infantryRem + cavalryRem + artilleryRem + 1);
            TroopType type;
            if(rand - artilleryRem <= 0)
            {
                type = TroopType.Artillery;
                artilleryRem--;
            }
            else if(rand - artilleryRem - cavalryRem < 0)
            {
                type = TroopType.Cavalry;
                cavalryRem--;
            }
            else
            {
                type = TroopType.Infantry;
                infantryRem--;
            }
            _deck.Push(new TerritoryCard(type, t.TerritoryName));
        }
        _deck.Push(new TerritoryCard(TroopType.WildCard, null));
        _deck.Push(new TerritoryCard(TroopType.WildCard, null));
        Shuffle();
    }
    // Helper method that shuffles the deck
    public void Shuffle()
    {
        List<TerritoryCard> list = new List<TerritoryCard>(_deck);
        for ( int i = 0; i < list.Count; i++ )
        {
            int num = Random.Range(0, list.Count);
            TerritoryCard temp = list[i];
            list[i] = list[num];
            list[num] = temp;
        }
        _deck = new Stack<TerritoryCard>(list);
    }
    public void Reset()
    {
        foreach(var card in _discardDeck)
        {
            _deck.Push(card);
        }
        _discardDeck.Clear();
        Shuffle();
    }
    public TerritoryCard DrawCard()
    {
        if(_deck.Count == 0)
        {
            Reset();
        }
        return _deck.Pop();
    }
    public void DiscardCard(TerritoryCard card)
    {
        _discardDeck.Push(card);
    }
}
