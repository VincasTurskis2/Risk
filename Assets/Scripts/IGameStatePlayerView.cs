using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IGameStatePlayerView
{
    public Player[] Players{get;}
    public Territory[] territories{get;}
    public int currentPlayerNo{get;}
    public TurnStage turnStage {get;}
    public UIManager uiManager {get;}
    public bool allTerritoriesClaimed {get;}
    public int cardSetRewardStage {get;}
    public Player CurrentPlayer();
}