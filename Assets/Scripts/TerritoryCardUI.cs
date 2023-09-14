using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class TerritoryCardUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _territoryNameText;
    [SerializeField]
    private Image _territoryImage;
    [SerializeField]
    private TextMeshProUGUI _troopTypeText;
    [SerializeField]
    private GameObject _wildcardText;
    [SerializeField]
    private TerritoryCard _cardData;

    private Color _selectedColor;

    private bool _selected;

    private Image _background;

    private CardUIManager _manager;
    
    public void Setup(TerritoryCard cardData, CardUIManager manager)
    {
        if(cardData == null || manager == null)
        {
            return;
        }
        _cardData = cardData;
        _manager = manager;
        if(_cardData.Type == TroopType.WildCard)
        {
            _territoryNameText.gameObject.SetActive(false);
            _territoryImage.gameObject.SetActive(false);
            _troopTypeText.gameObject.SetActive(false);
            _wildcardText.SetActive(true);
        }
        else
        {
            _territoryNameText.text = _cardData.ReferencedTerritory.TerritoryName;
            _territoryImage.sprite = _cardData.ReferencedTerritory.GetSprite();
            _troopTypeText.text = "" + _cardData.Type;

            _territoryNameText.gameObject.SetActive(true);
            _territoryImage.gameObject.SetActive(true);
            _troopTypeText.gameObject.SetActive(true);
            _wildcardText.SetActive(false);
        }
        _selected = false;
        _selectedColor = new Color(200f/255f, 1, 200f/255f, 1);
        _background = GetComponent<Image>();
    }

    public void ToggleSelect()
    {
        if(!_selected)
        {
            if(_manager.SelectCard(_cardData))
            {
                _selected = !_selected;
                _background.color = _selectedColor;
            }
        }
        else
        {
            if(_manager.UnselectCard(_cardData))
            {
                _selected = !_selected;
                _background.color = Color.white;
            }
        }
    }
}
