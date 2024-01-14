using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A script that's responsible for setting up the game
public class GameLoader : MonoBehaviour
{
    private GameMaster _gameState;
    private PlayerActions _playerActions;
    private UIManager _UIManager;
    void Awake()
    {
        _gameState = gameObject.GetComponent<GameMaster>();
        _playerActions = gameObject.GetComponent<PlayerActions>();
        _UIManager = gameObject.GetComponent<UIManager>();
        PlayerSelectionManager psm = GameObject.FindObjectOfType<PlayerSelectionManager>();
        if(psm == null)
        {
            _gameState.Setup();
        }
        else
        {
            _gameState.Setup(psm.playerData);
        }
        _playerActions.Setup();
        _UIManager.Setup();
        _gameState.Players()[0].StartTurn();
    }
}
