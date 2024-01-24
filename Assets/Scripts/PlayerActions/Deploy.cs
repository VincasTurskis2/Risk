public class Deploy : PlayerAction
{
    public readonly ITerritoryPlayerView ITerritory;

    public Deploy(Player Caller, ITerritoryPlayerView territory) : base(Caller)
    {
        ITerritory = territory;
    }
    public override bool execute()
    {    
        if(ITerritory == null) return false;
        TerritoryData territory = (TerritoryData) ITerritory;

        // Guards
        if(territory == null) return false;
        if(!GameMaster.Instance.state.getPlayerFromName(territory.Owner).IsMyTurn()) return false;
        if(GameMaster.Instance.state.turnStage != TurnStage.Deploy) return false;
        if(GameMaster.Instance.state.getPlayerFromName(territory.Owner).GetPlaceableTroopNumber() <= 0) return false;

        GameMaster.Instance.state.getPlayerFromName(territory.Owner).DecrementPlaceableTroops(1);
        territory.TroopCount++;
        if(GameMaster.Instance.state.getPlayerFromName(territory.Owner).GetPlaceableTroopNumber() <= 0)
        {
            GameMaster.Instance.state.getPlayerFromName(territory.Owner).EndTurnStage();
        }
        return true;
    }
}