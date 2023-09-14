using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _territoryCardPrefab;

    private List<TerritoryCardUI> _cardsDisplayed;
    private List<TerritoryCardUI> _cardsSelected;

    [SerializeField]
    private GameObject _centerCardPosition;
    [SerializeField]
    private Canvas _canvas;
    // Start is called before the first frame update
    void Start()
    {
        _cardsDisplayed = new();
        _cardsSelected = new();
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
        _cardsDisplayed.Clear();
        for(int i = 0; i < player.GetCardHand().Count; i++)
        {
            AddCard(player.GetCardHand()[i]);
        }
    }
    public bool SelectCard(TerritoryCardUI card)
    {
        if(card == null) return false;
        if(_cardsSelected.Count >= 3) return false;
        if(_cardsSelected.Contains(card)) return false;

        _cardsSelected.Add(card);
        return true;
    }
    public bool UnselectCard(TerritoryCardUI card)
    {
        if(card == null) return false;
        if(!_cardsSelected.Contains(card)) return true;

        _cardsSelected.Remove(card);
        return true;
    }
}
