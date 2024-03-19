using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        float ratio = (float) neighborTroopCount / territory.TroopCount;
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
            //Debug.Log(player.GetData().playerName + ": Deploying a troop to " + toDeploy.TerritoryName);
            success = new Deploy(player, toDeploy).execute();
            /*if(!success)
            {
                Debug.Log("Failed!");
            }*/
        }
        if (GameMaster.Instance.isSimulation)
        {
            state.turnStage = TurnStage.Attack;
        }
    }
    public static void Deploy_FavouriteContinent(GameState state, Player player)
    {
        int[] ownTroopsAtContinent = new int[6], totalTroopsAtContinent = new int[6];
        HashSet<TerritoryData>[] continentTerritories = new HashSet<TerritoryData>[6];
        for (int i = 0; i < 6; i++)
        {
            continentTerritories[i] = new();
        }
        foreach (TerritoryData territory in state.map.Territories)
        {
            Continent c = territory.Continent;
            continentTerritories[(int)c].Add(territory);
            totalTroopsAtContinent[(int)c] += territory.TroopCount;
            if(territory.Owner.Equals(player.GetData().playerName))
            {
                ownTroopsAtContinent[(int)c] += territory.TroopCount;
            }
            foreach (TerritoryData neighbour in state.map.GetRawTerritories(territory.GetNeighbors()))
            {
                if (neighbour.Continent != c)
                {
                    //continentTerritories[(int)c].Add(neighbour);
                    totalTroopsAtContinent[(int)c] += neighbour.TroopCount;
                    if (neighbour.Owner.Equals(player.GetData().playerName))
                    {
                        ownTroopsAtContinent[(int)c] += neighbour.TroopCount;
                    }
                }
            }
        }
        float[] ratios = new float[6];
        float maxNum = 0;
        int maxI = -1;
        for (int i = 0; i < 6; i++)
        {
            ratios[i] = (float) ownTroopsAtContinent[i] / totalTroopsAtContinent[i];
            if (ratios[i] > maxNum && ratios[i] != 1)
            {
                maxNum = ratios[i];
                maxI = i;
            }
        }

        //Deploy in most threatened border in the favourite continent
        bool success = true;
        var ownedTerritories = continentTerritories[maxI].Where(x => x.Owner.Equals(player.GetData().playerName)).ToArray();
        float maxRatio = 0;
        if (ownedTerritories.Length == 0)
        {
            Debug.Log(player.GetData().playerName + " has attempted to deploy troops, even though they have no territories");
            return;
        }
        ITerritoryPlayerView toDeploy = ownedTerritories[Random.Range(0, ownedTerritories.Length)];
        for (int i = 0; i < ownedTerritories.Length; i++)
        {
            if (ownedTerritories[i].Owner.Equals(player.GetData().playerName))
            {
                float ratio = TroopRatio(ownedTerritories[i], state, player);
                if (ratio > maxRatio)
                {
                    maxRatio = ratio;
                    toDeploy = ownedTerritories[i];
                }
            }
        }
        while (success)
        {
            success = new Deploy(player, toDeploy).execute();
        }
        if (GameMaster.Instance.isSimulation)
        {
            state.turnStage = TurnStage.Attack;
        }
    }

    public static bool Attack_RandomInFavouriteContinent(GameState state, Player player)
    {
        if(state.turnStage != TurnStage.Attack)
        {
            return player.IsCardEligible();
        }
        int[] ownTroopsAtContinent = new int[6], totalTroopsAtContinent = new int[6];
        HashSet<TerritoryData>[] continentTerritories = new HashSet<TerritoryData>[6];
        for (int i = 0; i < 6; i++)
        {
            continentTerritories[i] = new();
        }
        foreach (TerritoryData territory in state.map.Territories)
        {
            Continent c = territory.Continent;
            continentTerritories[(int)c].Add(territory);
            totalTroopsAtContinent[(int)c] += territory.TroopCount;
            if (territory.Owner.Equals(player.GetData().playerName))
            {
                ownTroopsAtContinent[(int)c] += territory.TroopCount;
            }
            foreach (TerritoryData neighbour in state.map.GetRawTerritories(territory.GetNeighbors()))
            {
                if (neighbour.Continent != c)
                {
                    continentTerritories[(int)c].Add(neighbour);
                    totalTroopsAtContinent[(int)c] += neighbour.TroopCount;
                    if (neighbour.Owner.Equals(player.GetData().playerName))
                    {
                        ownTroopsAtContinent[(int)c] += neighbour.TroopCount;
                    }
                }
            }
        }
        float[] ratios = new float[6];
        float maxNum = 0;
        int maxI = -1;
        for (int i = 0; i < 6; i++)
        {
            ratios[i] = (float)ownTroopsAtContinent[i] / totalTroopsAtContinent[i];
            if (ratios[i] > maxNum && ratios[i] != 1)
            {
                maxNum = ratios[i];
                maxI = i;
            }
        }
        // Attack randomly in the favourite continent
        bool cardEligible = false;
        List<Attack> legalAttacks = state.getAllPossibleAttacks(player);
        List<Attack> possibleAttacks = legalAttacks.Where(x => continentTerritories[maxI].Contains(x.ITo)).ToList();
        if (possibleAttacks.Count == 0)
        {
            possibleAttacks = legalAttacks;
        }
        possibleAttacks.Add(null);
        var rand = new System.Random();
        int index = rand.Next(0, possibleAttacks.Count);
        while (possibleAttacks[index] != null)
        {
            Attack attack = possibleAttacks[index];
            bool attackResult;
            do
            {
                attackResult = attack.execute();
            }
            while (attackResult == true);
            possibleAttacks = possibleAttacks.Where(x => x == null || x.IFrom.Equals(attack.IFrom) == false).ToList();
            if (attack.ITo.GetOwner().Equals(attack.IFrom.GetOwner()))
            {
                new Occupy(player, attack.IFrom, attack.ITo, attack.IFrom.TroopCount - 1).execute();
                cardEligible = true;
                if(attack.ITo.TroopCount > 1)
                {
                    foreach (var neighbour in state.map.GetRawTerritories(attack.ITo.GetNeighbors()))
                    {
                        if(!neighbour.Owner.Equals(player.GetData().playerName))
                        {
                            possibleAttacks.Add(new Attack(player, attack.ITo, neighbour));
                        }
                    }
                }
            }
            index = rand.Next(0, possibleAttacks.Count);
        }
        state.turnStage = TurnStage.Reinforce;
        Debug.Log("Attacking in the simulation");
        return cardEligible;
    }
}
