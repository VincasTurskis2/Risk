using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class SimulationHelper
{
    
    private float TroopRatio(ITerritoryPlayerView territory, GameState state, Player player)
    {
        int neighborTroopCount = NeighboringTroops(territory, state, player);
        float ratio = neighborTroopCount/territory.TroopCount;
        return ratio;
    }
    private int NeighboringTroops(ITerritoryPlayerView territory, GameState state, Player player)
    {
        int neighborTroopCount = 0;
        ITerritoryPlayerView[] neighbors = state.Map().GetTerritories(territory.GetNeighbors());
        foreach(var neighbor in neighbors)
        {
            if(!neighbor.GetOwner().Equals(player.GetData().playerName))
            {
                neighborTroopCount += neighbor.TroopCount;
            }
        }
        return neighborTroopCount;
    }
}
