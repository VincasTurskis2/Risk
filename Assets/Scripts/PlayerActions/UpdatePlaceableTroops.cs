using System.Collections.Generic;
using UnityEngine.PlayerLoop;

public class UpdatePlaceableTroops : PlayerAction
{
    public UpdatePlaceableTroops(Player Caller) : base(Caller)
    {

    }
    public override bool execute()
    {    
        if(GameMaster.Instance.state.turnStage == TurnStage.InitDeploy || GameMaster.Instance.state.turnStage == TurnStage.InitReinforce)
        {
            if(caller.GetPlaceableTroopNumber() == 0)
            {
                GameMaster.Instance.EndSetupStage();
                new UpdatePlaceableTroops(caller).execute();
            }
            return true;
        }
        else if (GameMaster.Instance.state.turnStage == TurnStage.Deploy)
        {
            int result = 0;
            result = GameMaster.Instance.state.map.GetOwnedTerritories(caller).Length / 3;
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
        foreach(TerritoryData t in GameMaster.Instance.state.map.GetOwnedTerritories(player))
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