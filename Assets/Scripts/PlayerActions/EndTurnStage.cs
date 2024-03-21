using System.Collections.Generic;
using UnityEngine;

public class EndTurnStage : PlayerAction
{

    public EndTurnStage(Player Caller) : base(Caller)
    {
    }
    public override bool execute()
    {
        if (!GameMaster.Instance.CurrentPlayer().GetData().playerName.Equals(caller.GetData().playerName))
        {
            Debug.Log("End turn stage failed: not your turn. Caller: " + caller.GetData().playerName + "; current turn: " + GameMaster.Instance.CurrentPlayer().GetData().playerName);
            return false;
        }
        switch(GameMaster.Instance.state.turnStage){
            case TurnStage.InitDeploy:
                break;
            case TurnStage.InitReinforce:
                break;
            case TurnStage.Deploy:
                if (caller.GetCardHand().Count >= 5)
                {
                    Debug.Log("End turn stage failed: Too many cards in hand");
                    return false;
                }
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