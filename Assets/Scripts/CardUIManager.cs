using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _territoryCardPrefab;

    private List<TerritoryCardUI> _cardsDisplayed;
    private List<TerritoryCard> _cardsSelected;

    [SerializeField]
    private GameObject _centerCardPosition;
    [SerializeField]
    private Canvas _canvas;
    [SerializeField]
    private Button _tradeInCardsButton;
    private PlayerActions _playerActions;


    public void Setup()
    {
        _cardsDisplayed = new();
        _cardsSelected = new();
        _tradeInCardsButton.interactable = false;
        _playerActions = (PlayerActions) FindAnyObjectByType(typeof(PlayerActions));
    }

    public void AddCard(TerritoryCard cardToAdd)
    {
        for(int i = 0; i < _cardsDisplayed.Count; i++)
        {
            RectTransform cardTransform = _cardsDisplayed[i].GetComponent<RectTransform>();
            _cardsDisplayed[i].transform.Translate(new Vector3(-(cardTransform.rect.width / 2 + 5) * _canvas.scaleFactor, 0, 0));
        }
        Vector3 lastCardPosition;
        if(_cardsDisplayed.Count == 0)
        {
            lastCardPosition = _centerCardPosition.transform.position;
            lastCardPosition.x -= (_territoryCardPrefab.GetComponent<RectTransform>().rect.width + 10) * _canvas.scaleFactor;
        }
        else
        {
            lastCardPosition = _cardsDisplayed[_cardsDisplayed.Count - 1].transform.position;
        }
        Vector3 newPosition = lastCardPosition;
        newPosition.x += (_territoryCardPrefab.GetComponent<RectTransform>().rect.width + 10) * _canvas.scaleFactor;
        GameObject instantiatedCard = Instantiate(_territoryCardPrefab, newPosition, Quaternion.identity, gameObject.transform);
        TerritoryCardUI newCard = instantiatedCard.GetComponent<TerritoryCardUI>();
        newCard.Setup(cardToAdd, this);
        _cardsDisplayed.Add(newCard);
    }
    public void RedrawCardHand(Player player)
    {
        foreach(TerritoryCardUI card in _cardsDisplayed)
        {
            Destroy(card.gameObject);
        }
        _cardsDisplayed.Clear();
        for(int i = 0; i < player.GetCardHand().Count; i++)
        {
            AddCard(player.GetCardHand()[i]);
        }
        _tradeInCardsButton.interactable = false;
    }
    public bool SelectCard(TerritoryCard card)
    {
        if(card == null) return false;
        if(_cardsSelected.Count >= 3) return false;
        if(_cardsSelected.Contains(card)) return false;

        _cardsSelected.Add(card);
        if(_cardsSelected.Count == 3)
        {
            _tradeInCardsButton.interactable = true;
        }
        return true;
    }
    public bool UnselectCard(TerritoryCard card)
    {
        if(card == null) return false;
        if(!_cardsSelected.Contains(card)) return true;

        _cardsSelected.Remove(card);
        _tradeInCardsButton.interactable = false;
        return true;
    }

    public Button GetTradeInCardsButton()
    {
        return _tradeInCardsButton;
    }
    public List<TerritoryCard> GetSelectedCards()
    {
        return _cardsSelected;
    }

    public int TradeInCards(Player player)
    {
        int result = _playerActions.TradeInCards(_cardsSelected.ToArray(), player);
        if(result != 0)
        {
            _cardsSelected.Clear();
            RedrawCardHand(player);
        }
        return result;
    }
}
