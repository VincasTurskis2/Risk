using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logger : MonoBehaviour
{
    private Logger()
    {
    }
    public static Logger Instance { get; private set; }
    public List<PlayerLoggingData> playerData = new();
    public List<GameLoggingData> gameData = new();

    public void Awake()
    {
        Instance = this;
    }
    public void SetupLogger(PlayerData[] players)
    {
        playerData.Clear();
        gameData.Clear();
        foreach (var p in players)
        {
            playerData.Add(new PlayerLoggingData(p));
        }
    }
    public void RecordIterationResults(string winner, float gameTime, GameState state)
    {
        gameData.Add(new GameLoggingData(winner, gameTime, GameMaster.Instance.turnCount));
        getByName(winner).wins++;
    }
    public void RecordTurnData(string player, float duration, int numberOfAttacks)
    {
        var pd = getByName(player);
        pd.cumulativeTurnDuration += duration;
        pd.cumulativeNoOfTurns++;
        pd.cumulativeNoOfAttacks += numberOfAttacks;
    }
    public void OutputSimulationResults()
    {
        var a = playerData;
        var b = gameData;
        Debug.Log("Printing simulation results");
    }
    public class PlayerLoggingData
    {
        public int wins;
        public string playerName;
        public float cumulativeTurnDuration;
        public int cumulativeNoOfTurns;
        public int cumulativeNoOfAttacks;
        public PlayerLoggingData(PlayerData player) {
            playerName = player.playerName;
            cumulativeTurnDuration = 0;
            cumulativeNoOfTurns = 0;
            wins = 0;
            cumulativeNoOfAttacks = 0;
        }
    }
    public PlayerLoggingData getByName(string name)
    {
        foreach (var p in playerData)
        {
            if (p.playerName.Equals(name))
            {
                return p;
            }
        }
        return null;
    }

    public class GameLoggingData
    {
        public string winner;
        public float gameTimeSeconds;
        public int turnCount;
        public GameLoggingData(string Winner, float GameTimeSeconds, int TurnCount)
        {
            winner = Winner;
            gameTimeSeconds = GameTimeSeconds;
            turnCount = TurnCount;
        }
    }
}
