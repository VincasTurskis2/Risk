public class Fortify : PlayerAction
{
    public readonly ITerritoryPlayerView IFrom;
    public readonly ITerritoryPlayerView ITo;
    public readonly int numberOfTroops;

    public Fortify(Player Caller, ITerritoryPlayerView from, ITerritoryPlayerView to, int NumberOfTroops) : base(Caller)
    {

        IFrom = from;
        ITo = to;
        numberOfTroops = NumberOfTroops;
    }
    public override bool execute()
    {    
        if(IFrom == null || ITo == null) return false;
        TerritoryData from = (TerritoryData) IFrom, to = (TerritoryData) ITo;

        // Guards
        if(from == null || to == null || numberOfTroops < 1) return false;
        if(!GameMaster.Instance.state.getPlayerFromName(from.Owner).IsMyTurn()) return false;
        if(numberOfTroops >= from.TroopCount) return false;
        if(!from.IsANeighbor(to)) return false;
        if(!to.Owner.Equals(from.Owner)) return false;
        if(GameMaster.Instance.state.turnStage != TurnStage.Reinforce) return false;

        // If all the guards are passed, move the troops
        from.TroopCount -= numberOfTroops;
        to.TroopCount += numberOfTroops;
        GameMaster.Instance.state.getPlayerFromName(from.Owner).EndTurnStage(); // Only 1 reinforcement can be done per turn
        return true;
    }
}