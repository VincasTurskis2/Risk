using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TroopType{
    Infantry = 1, Cavalry = 2, Artillery = 4, WildCard = 7
}
public class TerritoryCard
{
    [field: SerializeField]
    public TroopType Type {get; set;}
    [field: SerializeField]
    public TerritoryData ReferencedTerritory {get; set;}


    public static readonly int[] CardSetRewards = {4, 6, 8, 10, 12, 15};

    public TerritoryCard(TroopType type, TerritoryData reference)
    {
        Type = type;
        ReferencedTerritory = reference;
    }
}
