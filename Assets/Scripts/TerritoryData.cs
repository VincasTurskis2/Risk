using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerritoryData : ITerritoryPlayerView
{
    public Color territoryColor {get; set;}

    public readonly Sprite sprite;

    // Immutable properties of the territory
    public string TerritoryName {get;}
    public string[] Neighbors {get;}
    public Continent Continent {get;}

    // Mutable properties of the territory
    
    public int TroopCount {get; set;}
    public Player Owner {get; private set;}

    public TerritoryData(Sprite newSprite, string territoryName, Territory[] neighbors, Continent continent, int troopCount, Player owner)
    {
        territoryColor = new Color(1, 1, 1, 1);
        sprite = newSprite;
        TerritoryName = territoryName;
        Neighbors = new string[neighbors.Length];
        for (int i = 0; i < neighbors.Length; i++)
        {
            Neighbors[i] = neighbors[i].TerritoryName;
        }
        Continent = continent;
        TroopCount = troopCount;
        Owner = owner;
    }

    public bool IsANeighbor(TerritoryData other)
    {
        bool contains = false;
        for(int i = 0;+ i < Neighbors.Length; i++)
        {
            if(Neighbors[i] == other.TerritoryName) 
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
            if(Neighbors[i] == other.TerritoryName) 
            {
                contains = true;
                break;
            }
        }
        return contains;
    }
    public void SetOwner(Player newOwner, bool setCardEligible)
    {
        if(Owner != null)
        {
            Owner.GetOwnedTerritories().Remove(this);
        }
        if(newOwner != null && Owner != newOwner && setCardEligible)
        {
            newOwner.SetCardEligible(true);
        }
        Owner = newOwner;
        if(Owner != null)
        {
            Owner.GetOwnedTerritories().Add(this);
            territoryColor = Owner.GetData().playerColor;
        }
    }
    public void Highlight(bool toHighlight)
    {
        if(Owner == null) return;
        if(toHighlight)
        {
            territoryColor = Helpers.GetHighlighedColorVersion(Owner.GetData().playerColor);
        }
        else
        {
            territoryColor = Owner.GetData().playerColor;
        }
    }
    public string[] GetNeighbors()
    {
        return Neighbors;
    }

    public IOtherPlayer GetOwner()
    {
        return Owner;
    }
}
