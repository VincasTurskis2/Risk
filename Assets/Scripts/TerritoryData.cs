using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerritoryData : ITerritoryPlayerView
{
    private GameMaster _gameState;

    private SpriteRenderer _renderer;

    // Immutable properties of the territory
    public string TerritoryName {get; private set;}
    public TerritoryData[] Neighbors {get; private set;}
    public Continent Continent {get; private set;}

    // Mutable properties of the territory
    
    public int TroopCount {get; set;}
    public Player Owner {get; private set;}

    public TerritoryData(GameMaster gameState, SpriteRenderer renderer, string territoryName, Territory[] neighbors, Continent continent, int troopCount, Player owner)
    {
        _gameState = gameState;
        _renderer = renderer;
        TerritoryName = territoryName;
        Neighbors = new TerritoryData[neighbors.Length];
        Continent = continent;
        TroopCount = troopCount;
        Owner = owner;
    }
    public void SetupNeighbors(Territory[] neighbors)
    {
        for (int i = 0; i < neighbors.Length; i++)
        {
            Neighbors[i] = neighbors[i].data;
        }
    }

    public bool IsANeighbor(TerritoryData other)
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
        TerritoryData other = (TerritoryData) IOther;
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
        if(newOwner != null && Owner != newOwner && _gameState.turnStage() != TurnStage.Setup)
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
