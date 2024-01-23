using System.Collections.Generic;
using UnityEngine;
public class SetupDeploy : PlayerAction
{
    public readonly ITerritoryPlayerView ITerritory;

    public SetupDeploy(Player Caller, IGameMasterPlayerView GameMaster, ITerritoryPlayerView territory) : base(Caller, GameMaster)
    {
        ITerritory = territory;
    }
    public override bool execute()
    {    
        if(ITerritory == null) return false;
        TerritoryData territory = (TerritoryData) ITerritory;


        //Guards
        if(territory == null) return false;
        if(gameMaster.turnStage() != TurnStage.Setup) return false;
        if(!caller.IsMyTurn()) return false;
        if(caller.GetPlaceableTroopNumber() < 1) return false;

        if((Object)territory.Owner == null)
        {
            territory.SetOwner(caller, false);
            territory.TroopCount++;
            caller.DecrementPlaceableTroops(1);
            caller.EndTurn();
            return true;
        }
        else if((Object)territory.Owner == (Object) caller && gameMaster.allTerritoriesClaimed)
        {
            territory.TroopCount++;
            caller.DecrementPlaceableTroops(1);
            caller.EndTurn();
            return true;
        }
        else return false;
    }
}