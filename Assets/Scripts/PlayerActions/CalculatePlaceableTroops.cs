using System.Collections.Generic;

public class UpdatePlaceableTroops : PlayerAction
{
    public UpdatePlaceableTroops(Player Caller, GameMaster GameMaster) : base(Caller, GameMaster)
    {

    }
    public override bool execute()
    {    
        if(gameMaster.turnStage() == TurnStage.Setup)
        {
            if(caller.GetPlaceableTroopNumber() == 0)
            {
                gameMaster.EndSetupStage();
            }
            return true;
        }
        else if (gameMaster.turnStage() == TurnStage.Deploy)
        {
            int result = 0;
            result = caller.GetOwnedTerritories().Count / 3;
            List<Continent> ownedContinents = GetOwnedContinents(caller);
            for(int i = 0; i < ownedContinents.Count; i++)
            {
                result += GameMaster.ContinentValues[(int) ownedContinents[i]];
            }
            caller.SetPlaceableTroopNumber(result);
            return true;
        }
        else return false;
    }
    private List<Continent> GetOwnedContinents(Player player)
    {
        List<Continent> result = new List<Continent>();
        int[] continentCountLocal = new int[GameMaster.ContinentCount.Length];
        GameMaster.ContinentCount.CopyTo(continentCountLocal, 0);
        foreach(TerritoryData t in player.GetOwnedTerritories())
        {
            continentCountLocal[(int) t.Continent]--;
        }
        for(int i = 0; i < continentCountLocal.Length; i++)
        {
            if(continentCountLocal[i] <= 0)
            {
                result.Add((Continent) i);
            }
        }
        return result;
    }
}