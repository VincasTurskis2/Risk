using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    [field: SerializeField]
    public string playerName {get; private set;}
    [field: SerializeField]
    public PlayerType playerType{get; private set;}
    [field: SerializeField]
    public Color playerColor {get; private set;}

    public PlayerData(string name, PlayerType type, Color color)
    {
        playerName = name;
        playerType = type;
        playerColor = color;
    }
}