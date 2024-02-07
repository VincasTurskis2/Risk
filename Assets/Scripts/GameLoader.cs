using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A script that's responsible for setting up the game
public class GameLoader : MonoBehaviour
{
    private UIManager _UIManager;
    void Awake()
    {
        _UIManager = gameObject.GetComponent<UIManager>();
        PlayerSelectionManager psm = FindObjectOfType<PlayerSelectionManager>();
        if(psm == null)
        {
            GameMaster.Instance.Setup();
        }
        else
        {
            GameMaster.Instance.Setup(psm.playerData);
        }
        _UIManager.Setup();
        GameMaster.Instance.state.players[0].StartTurn();
    }
}
