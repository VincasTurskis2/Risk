public class DeployMultiple : PlayerAction
{
    public readonly ITerritoryPlayerView[] ITerritories;
    int[] amounts;

    public DeployMultiple(Player Caller, ITerritoryPlayerView[] Territories, int[] Amounts) : base(Caller)
    {
        ITerritories = Territories;
        amounts = Amounts;

    }
    public override bool execute()
    {    
        // Guards
        if(ITerritories == null || amounts == null) return false;
        if(amounts.Length != ITerritories.Length) return false;
        TerritoryData[] territories = new TerritoryData[ITerritories.Length];

        if(GameMaster.Instance.state.turnStage != TurnStage.Deploy) return false;
        int totalToDeploy = 0;
        Player owner = null;
        for(int i = 0; i < ITerritories.Length; i++)
        {
            territories[i] = (TerritoryData) ITerritories[i];
            if(owner == null)
            {
                if(!GameMaster.Instance.state.getPlayerFromName(territories[i].Owner).IsMyTurn()) return false;
                owner = GameMaster.Instance.state.getPlayerFromName(territories[i].Owner);
            }
            else 
            {
                if(!territories[i].Owner.Equals(owner.GetData().playerName)) return false;
            }
            totalToDeploy += amounts[i];
        }
        if(owner == null) return false;
        if(totalToDeploy > owner.GetPlaceableTroopNumber()) return false;
        for(int i = 0; i < territories.Length; i++)
        {
            GameMaster.Instance.state.getPlayerFromName(territories[i].Owner).DecrementPlaceableTroops(amounts[i]);
            territories[i].TroopCount += amounts[i];
        }
        if(owner.GetPlaceableTroopNumber() <= 0)
        {
            owner.EndTurnStage();
        }
        return true;
    }
}