using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class TerritoryUI : MonoBehaviour
{
    private TerritoryData _territory;
    [SerializeField]
    private TextMeshProUGUI _troopCountText;
    [SerializeField]
    private TextMeshProUGUI _territoryNameText;
    void Start()
    {
        _territory = gameObject.GetComponent<Territory>().data;
        _territoryNameText.text = _territory.TerritoryName;
        
    }

    void Update()
    {
        _troopCountText.SetText(_territory.TroopCount.ToString());
    }
}
