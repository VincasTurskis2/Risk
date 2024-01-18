using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// A class to represent a single territory
public class Territory : MonoBehaviour
{
    private SpriteRenderer _renderer;

    // Immutable properties of the territory
    [field: SerializeField]
    public string TerritoryName {get; private set;}
    [field: SerializeField]
    public Territory[] Neighbors {get; private set;}
    [field: SerializeField]
    public Continent Continent {get; private set;}

    // Mutable properties of the territory
    
    [field: SerializeField]
    public int TroopCount {get; set;}
    public Player Owner {get; private set;}

    public TerritoryData data {get; set;}

    public void Setup()
    {
        _renderer = GetComponent<SpriteRenderer>();
        data = new TerritoryData(_renderer, TerritoryName, Neighbors, Continent, TroopCount, Owner);

    }
}