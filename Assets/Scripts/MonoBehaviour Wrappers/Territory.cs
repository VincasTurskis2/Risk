using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// A class to represent a single territory
public class Territory : MonoBehaviour
{

    private GameMaster _gameState;

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

    public TerritoryData data;

    public void Setup()
    {
        _gameState = (GameMaster) FindAnyObjectByType(typeof(GameMaster));
        _renderer = GetComponent<SpriteRenderer>();
        data = new TerritoryData(_gameState, _renderer, TerritoryName, Neighbors, Continent, TroopCount, Owner);

    }
    public void SetupNeighbors()
    {
        data.SetupNeighbors(Neighbors);
    }
}