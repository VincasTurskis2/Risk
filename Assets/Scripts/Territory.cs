using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// A class to represent a single territory
public class Territory : MonoBehaviour, ITerritoryPlayerView
{

    private GameState _gameState;

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

    public void Setup()
    {
        _gameState = (GameState) FindAnyObjectByType(typeof(GameState));
        _renderer = GetComponent<SpriteRenderer>();
    }
    public bool IsANeighbor(Territory other)
    {
        bool contains = false;
        for(int i = 0;+ i < Neighbors.Length; i++)
        {
            if(Neighbors[i] == other) 
            {
                contains = true;
                break;
            }
        }
        return contains;
    }
    public bool IsANeighbor(ITerritoryPlayerView IOther)
    {
        if(IOther == null) return false;
        Territory other = (Territory) IOther;
        if(other == null) return false;
        bool contains = false;
        for(int i = 0;+ i < Neighbors.Length; i++)
        {
            if(Neighbors[i] == other) 
            {
                contains = true;
                break;
            }
        }
        return contains;
    }
    public void SetOwner(Player newOwner)
    {
        if(Owner != null)
        {
            Owner.GetOwnedTerritories().Remove(this);
        }
        if(newOwner != null && Owner != newOwner && _gameState.turnStage != TurnStage.Setup)
        {
            newOwner.SetCardEligible(true);
        }
        Owner = newOwner;
        if(Owner != null)
        {
            Owner.GetOwnedTerritories().Add(this);
            _renderer.color = Owner.GetData().playerColor;
        }
    }
    public void Highlight(bool toHighlight)
    {
        if(Owner == null) return;
        if(toHighlight)
        {
            _renderer.color = Helpers.GetHighlighedColorVersion(Owner.GetData().playerColor);
        }
        else
        {
            _renderer.color = Owner.GetData().playerColor;
        }
    }
    public Sprite GetSprite()
    {
        return _renderer.sprite;
    }

    public ITerritoryPlayerView[] GetNeighbors()
    {
        return Neighbors;
    }

    public IOtherPlayer GetOwner()
    {
        return Owner;
    }
}