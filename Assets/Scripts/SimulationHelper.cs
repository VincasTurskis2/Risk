using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public static class SimulationHelper
{
    public static void SimulationDeploy(Player player, GameState state)
    {
        new TradeInAnyCards(player).execute();
        ITerritoryPlayerView[] myTerritories = state.Map().GetOwnedTerritories(player);
        while(player.GetPlaceableTroopNumber() > 0)
        {
            float maxRatio = 0;
            ITerritoryPlayerView toDeploy = myTerritories[UnityEngine.Random.Range(0, myTerritories.Length)];
            for(int i = 0; i < myTerritories.Length; i++)
            {
                float ratio = TroopRatio(myTerritories[i], state, player);
                if(ratio > maxRatio)
                {
                    maxRatio = ratio;
                    toDeploy = myTerritories[i];
                }
            }
            new Deploy(player, toDeploy).execute();
        }
        if(state.turnStage == TurnStage.Deploy)
        {
            state.turnStage = TurnStage.Attack;
        }
    }
    private static float TroopRatio(ITerritoryPlayerView territory, GameState state, Player player)
    {
        int neighborTroopCount = NeighboringTroops(territory, state, player);
        float ratio = neighborTroopCount/territory.TroopCount;
        return ratio;
    }
    private static int NeighboringTroops(ITerritoryPlayerView territory, GameState state, Player player)
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


    public static void SimulationAttack(Player player, GameState state)
    {
        List<Attack> possibleAttacks = state.getAllPossibleAttacks();
        var rand = new System.Random();
        while(rand.Next(-1, possibleAttacks.Count) != -1)
        {
            int randIndex = rand.Next(0, possibleAttacks.Count);
            Attack attack = possibleAttacks[randIndex];
            bool attackResult;
            do
            {
                attackResult = attack.execute();
            }
            while(attackResult == true);

            if(attack.ITo.GetOwner().Equals(attack.IFrom.GetOwner()))
            {
                new Occupy(player, attack.IFrom, attack.ITo, attack.IFrom.TroopCount - 1);
            }
            possibleAttacks = state.getAllPossibleAttacks();
        }
        state.turnStage = TurnStage.Reinforce;
    }


    public static void SimulationReinforce(Player player, GameState state)
    {
        ITerritoryPlayerView[] myTerritories = state.Map().GetOwnedTerritories(player);
        ITerritoryPlayerView strongestInlandTerritory = null;
        foreach(var territory in myTerritories)
        {
            if(territory.TroopCount > 1 && NeighboringTroops(territory, state, player) == 0)
            {
                if(strongestInlandTerritory == null)
                {
                    strongestInlandTerritory = territory;
                }
                else if(strongestInlandTerritory.TroopCount < territory.TroopCount)
                {
                    strongestInlandTerritory = territory;
                }
            }
        }
        if(strongestInlandTerritory == null)
        {
            state.turnStage = TurnStage.Deploy;
            return;
        }
        ITerritoryPlayerView[] neighbors = state.Map().GetTerritories(strongestInlandTerritory.GetNeighbors());
        float maxRatio = 0;
        ITerritoryPlayerView toFortify = null;
        foreach(var neighbor in neighbors)
        {
            float ratio = TroopRatio(neighbor, state, player);
            if(ratio > maxRatio)
            {
                maxRatio = ratio;
                toFortify = neighbor;
            }
        }
        if(toFortify == null)
        {
            state.turnStage = TurnStage.Deploy;
            return;
        }
        new Fortify(player, strongestInlandTerritory, toFortify, strongestInlandTerritory.TroopCount - 1).execute(false);
        state.turnStage = TurnStage.Deploy;
    }
}
