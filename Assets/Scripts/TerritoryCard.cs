using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TroopType{
    Infantry = 1, Cavalry = 2, Artillery = 4, WildCard = 7
}
public class TerritoryCard
{
    public TroopType Type {get; set;}
    public string ReferencedTerritory {get;}

    public TerritoryCard(TroopType type, string reference)
    {
        Type = type;
        ReferencedTerritory = reference;
    }

    public TerritoryCard(TerritoryCard oldCard)
    {
        Type = oldCard.Type;
        ReferencedTerritory = oldCard.ReferencedTerritory;
    }
}
