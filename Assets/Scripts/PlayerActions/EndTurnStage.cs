using System.Collections.Generic;

public class EndTurnStage : PlayerAction
{

    public EndTurnStage(Player Caller, IGameStatePlayerView GameMaster) : base(Caller, GameMaster)
    {
    }
    public override bool execute()
    {    
        if(gameMaster.CurrentPlayer() != caller) return false;
        switch(gameMaster.turnStage()){
            case TurnStage.Setup:
                break;
            case TurnStage.Deploy:
                if(caller.GetCardHand().Count >= 5) return false;
                gameMaster.state.turnStage = TurnStage.Attack;
                break;
            case TurnStage.Attack:
                gameMaster.state.turnStage = TurnStage.Reinforce;
                break;
            case TurnStage.Reinforce:
                caller.EndTurn();
                break;
        }
        return true;
    }
}