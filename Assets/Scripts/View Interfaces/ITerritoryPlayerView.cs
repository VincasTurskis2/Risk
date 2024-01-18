using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface ITerritoryPlayerView
{
    public string TerritoryName {get;}
    public string[] GetNeighbors();
    public Continent Continent{get;}
    public bool IsANeighbor(ITerritoryPlayerView other);
    public void Highlight(bool toHighlight);
    public Sprite GetSprite();
    public int TroopCount{get;}
    public IOtherPlayer GetOwner();
}
