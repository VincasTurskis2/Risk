using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    [field: SerializeField]
    public string playerName {get;}
    [field: SerializeField]
    public PlayerType playerType{get;}
    [field: SerializeField]
    public Color playerColor {get;}

    public PlayerData(string name, PlayerType type, Color color)
    {
        playerName = name;
        playerType = type;
        playerColor = color;
    }
}