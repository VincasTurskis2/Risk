using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TerritoryUI : MonoBehaviour
{
    private Territory _territory;
    [field: SerializeField]
    public TextMeshProUGUI TroopCountText {get; private set;}
    void Start()
    {
        _territory = this.gameObject.GetComponent<Territory>();
    }

    void Update()
    {
        TroopCountText.SetText(_territory.TroopCount.ToString());
    }
}
