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
    
    public void Setup(TerritoryCard cardData)
    {
        if(cardData == null)
        {
            return;
        }
        _cardData = cardData;
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
