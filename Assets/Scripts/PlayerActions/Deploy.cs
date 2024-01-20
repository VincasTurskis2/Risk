public class Deploy : PlayerAction
{
    public readonly ITerritoryPlayerView ITerritory;

    public Deploy(Player Caller, GameMaster GameMaster, ITerritoryPlayerView territory) : base(Caller, GameMaster)
    {
        ITerritory = territory;
    }
    public override bool execute()
    {    
        if(ITerritory == null) return false;
        TerritoryData territory = (TerritoryData) ITerritory;

        // Guards
        if(territory == null) return false;
        if(!territory.Owner.IsMyTurn()) return false;
        if(gameMaster.turnStage() != TurnStage.Deploy) return false;
        if(territory.Owner.GetPlaceableTroopNumber() <= 0) return false;

        territory.Owner.DecrementPlaceableTroops(1);
        territory.TroopCount++;
        if(territory.Owner.GetPlaceableTroopNumber() <= 0)
        {
            territory.Owner.EndTurnStage();
        }
        return true;
    }
}