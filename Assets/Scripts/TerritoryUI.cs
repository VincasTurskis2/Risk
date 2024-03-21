using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class TerritoryUI : MonoBehaviour
{

    private SpriteRenderer _renderer;
    private TerritoryData _territory;
    [SerializeField]
    private TextMeshProUGUI _troopCountText;
    [SerializeField]
    private TextMeshProUGUI _territoryNameText;
    void Start()
    {
        _renderer = gameObject.GetComponent<SpriteRenderer>();
        _territory = gameObject.GetComponent<Territory>().data;
        _territoryNameText.text = _territory.TerritoryName;
    }
    public void Setup()
    {
        _territory = gameObject.GetComponent<Territory>().data;
    }

    void Update()
    {
        if(!GameMaster.Instance.state.simulationState)
        {
           _troopCountText.SetText(_territory.TroopCount.ToString());
           _renderer.color = _territory.territoryColor;
        }
    }
}
