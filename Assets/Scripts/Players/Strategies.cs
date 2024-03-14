using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Strategies
{
    private static int NeighboringTroops(ITerritoryPlayerView territory, GameState gameState, Player player)
    {
        int neighborTroopCount = 0;
        ITerritoryPlayerView[] neighbors = gameState.Map().GetTerritories(territory.GetNeighbors());
        foreach (var neighbor in neighbors)
        {
            if (!neighbor.GetOwner().Equals(player.GetData().playerName))
            {
                neighborTroopCount += neighbor.TroopCount;
            }
        }
        return neighborTroopCount;
    }
    private static float TroopRatio(ITerritoryPlayerView territory, GameState gameState, Player player)
    {
        int neighborTroopCount = NeighboringTroops(territory, gameState, player);
        float ratio = neighborTroopCount / territory.TroopCount;
        return ratio;
    }
    public static void Deploy_MostThreatenedBorder(GameState state, Player player)
    {
        ITerritoryPlayerView[] myTerritories = state.Map().GetOwnedTerritories(player);
        bool success = true;
        while (success)
        {
            float maxRatio = 0;
            ITerritoryPlayerView toDeploy = myTerritories[UnityEngine.Random.Range(0, myTerritories.Length)];
            for (int i = 0; i < myTerritories.Length; i++)
            {
                float ratio = TroopRatio(myTerritories[i], state, player);
                if (ratio > maxRatio)
                {
                    maxRatio = ratio;
                    toDeploy = myTerritories[i];
                }
            }
            success = new Deploy(player, toDeploy).execute();
        }
        if (state.turnStage == TurnStage.Deploy)
        {
            Debug.Log("Manually ending turn stage");
            new EndTurnStage(player).execute();
        }
    }
    public static void Deploy_FavouriteContinent(GameState state, Player player)
    {
        int[] ownTroopsAtContinent = new int[6], totalTroopsAtContinent = new int[6];
        foreach (TerritoryData territory in state.map.Territories)
        {
            Continent c = territory.Continent;
            totalTroopsAtContinent[(int)c] += territory.TroopCount;
            if(territory.Owner.Equals(player.GetData().playerName))
            {
                ownTroopsAtContinent[(int)c] += territory.TroopCount;
            }
        }
    }
}
