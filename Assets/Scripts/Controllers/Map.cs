using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Map : IMapPlayerView
{
    public TerritoryData[] Territories { get; private set;}

    public Map(Territory[] territories)
    {
        Territories = new TerritoryData[territories.Length];
        for (int i = 0 ; i < Territories.Length ; i++)
        {
            territories[i].Setup();
            Territories[i] = territories[i].data; 
        }
    }

    public Map(Map oldMap)
    {
        
    }

    public TerritoryData[] GetRawTerritories(string[] territories)
    {
        IEnumerable<TerritoryData> result = from t in Territories
                                            where territories.Contains(t.TerritoryName)
                                            select t;    
        return result.ToArray();
    }

    public TerritoryData GetRawTerritory(string territory)
    {
        foreach(TerritoryData t in Territories)
        {
            if (t.TerritoryName.Equals(territory))
            {
                return t;
            }
        }
        return null;
    }
    public ITerritoryPlayerView[] GetTerritories()
    {
        return Territories;
    }
    public ITerritoryPlayerView[] GetTerritories(string[] territories)
    {
        return GetRawTerritories(territories);
    }
    public ITerritoryPlayerView GetTerritory(string territory)
    {
        return GetRawTerritory(territory);
    }
}