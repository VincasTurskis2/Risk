using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _territoryCardPrefab;

    private List<TerritoryCardUI> _cardsDisplayed;

    [SerializeField]
    private GameObject _centerCardPosition;
    [SerializeField]
    private Canvas _canvas;
    // Start is called before the first frame update
    void Start()
    {
        _cardsDisplayed = new();
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
        newCard.Setup(cardToAdd);
        _cardsDisplayed.Add(newCard);
    }
    /*public void AddNewMenu()
    {
        Vector3 newPosition = PlayerMenus[PlayerMenus.Count - 1].transform.position;
        newPosition.y -= 60 * _canvas.scaleFactor;
        GameObject instantiatedMenu = Instantiate(_playerMenuPrefab, newPosition, Quaternion.identity, _playerSelectionPanel.transform);
        PlayerSelectorMenu newMenu = instantiatedMenu.GetComponent<PlayerSelectorMenu>();
        newMenu.Setup(this);
        PlayerMenus.Add(newMenu);
        _addPlayerButton.transform.Translate(new Vector3(0, -60 * _canvas.scaleFactor, 0));
        _playerCountText.text = "Players (" + PlayerMenus.Count + "/6)";
        CheckButtonEnable();
        
    }*/
}
