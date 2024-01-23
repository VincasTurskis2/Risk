using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField]
    private Button _showHideCardsButton;
    [SerializeField]
    private TextMeshProUGUI _showHideCardsButtonText;
    [SerializeField]
    private GameObject _cardsHiddenText;

    public GameMaster gameMaster{get; set;}

    private bool _cardsHidden;


    public void Setup(GameMaster state)
    {
        _cardsHidden = false;
        _cardsHiddenText.SetActive(false);
        _showHideCardsButtonText.text = "Hide Cards";
        _cardsDisplayed = new();
        _cardsSelected = new();
        _tradeInCardsButton.interactable = false;
        gameMaster = state;
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
        _cardsSelected.Clear();
        if(_cardsHidden)
        {
            _cardsHiddenText.SetActive(true);
        }
        else
        {
            _cardsHiddenText.SetActive(false);
            for(int i = 0; i < player.GetCardHand().Count; i++)
            {
                AddCard(player.GetCardHand()[i]);
            }
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

    public bool TradeInCards(Player player)
    {
        bool result = new TradeInCards(player, gameMaster, _cardsSelected.ToArray()).execute();
        if(result)
        {
            _cardsSelected.Clear();
            RedrawCardHand(player);
        }
        return result;
    }

    public void ToggleShowCards()
    {
        if(_cardsHidden)
        {
            _showHideCardsButtonText.text = "Hide Cards";
            _cardsHidden = false;
        }
        else
        {
            _showHideCardsButtonText.text = "Show Cards";
            _cardsHidden = true;
        }
        RedrawCardHand(gameMaster.CurrentPlayer());
    }
}
