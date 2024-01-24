using System.Collections.Generic;

public class EndTurnStage : PlayerAction
{

    public EndTurnStage(Player Caller) : base(Caller)
    {
    }
    public override bool execute()
    {    
        if(GameMaster.Instance.CurrentPlayer() != caller) return false;
        switch(GameMaster.Instance.state.turnStage){
            case TurnStage.InitDeploy:
                break;
            case TurnStage.InitReinforce:
                break;
            case TurnStage.Deploy:
                if(caller.GetCardHand().Count >= 5) return false;
                GameMaster.Instance.state.turnStage = TurnStage.Attack;
                break;
            case TurnStage.Attack:
                GameMaster.Instance.state.turnStage = TurnStage.Reinforce;
                break;
            case TurnStage.Reinforce:
                caller.EndTurn();
                break;
        }
        return true;
    }
}