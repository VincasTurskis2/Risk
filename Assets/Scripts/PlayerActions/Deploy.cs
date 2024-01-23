public class Deploy : PlayerAction
{
    public readonly ITerritoryPlayerView ITerritory;

    public Deploy(Player Caller, IGameMasterPlayerView GameMaster, ITerritoryPlayerView territory) : base(Caller, GameMaster)
    {
        ITerritory = territory;
    }
    public override bool execute()
    {    
        if(ITerritory == null) return false;
        TerritoryData territory = (TerritoryData) ITerritory;

        // Guards
        if(territory == null) return false;
        if(!gameMaster.getPlayerFromName(territory.Owner).IsMyTurn()) return false;
        if(gameMaster.turnStage() != TurnStage.Deploy) return false;
        if(gameMaster.getPlayerFromName(territory.Owner).GetPlaceableTroopNumber() <= 0) return false;

        gameMaster.getPlayerFromName(territory.Owner).DecrementPlaceableTroops(1);
        territory.TroopCount++;
        if(gameMaster.getPlayerFromName(territory.Owner).GetPlaceableTroopNumber() <= 0)
        {
            gameMaster.getPlayerFromName(territory.Owner).EndTurnStage();
        }
        return true;
    }
}