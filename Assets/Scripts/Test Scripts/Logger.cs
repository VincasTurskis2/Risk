using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Logger : MonoBehaviour
{
    private Logger()
    {
    }
    public static Logger Instance { get; private set; }
    public List<PlayerLoggingData> playerData = new();
    public List<GameLoggingData> gameData = new();
    private string path;
    public void Awake()
    {
        Instance = this;
    }
    private void write(string toWrite)
    {
        File.AppendAllText(path, toWrite+"\n");
    }
    public void SetupLogger(PlayerData[] players)
    {
        int nextNo = 0;
        while(File.Exists(Application.dataPath + "/Logs/sim_log_" + nextNo + ".txt"))
        {
            nextNo++;
        }
        path = Application.dataPath + "/Logs/sim_log_" + nextNo + ".txt";
        File.WriteAllText(path, "Simulation log: \n\n");
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
        write("Number of iterations: " + gameData.Count);
        write("Player data:");
        foreach (var pd in playerData)
        {
            write(pd.playerName + ":");
            if(pd.cumulativeNoOfTurns > 0)
            {
                write(" Average turn duration: " + pd.cumulativeTurnDuration / pd.cumulativeNoOfTurns + "s.");
                write(" Average attacks per turn: " + (float)pd.cumulativeNoOfAttacks / pd.cumulativeNoOfTurns);
            }
            write(" Winrate: " + (float)pd.wins * 100 / gameData.Count +"%");
        }
        write("\nGameData:");
        {
            foreach (var gd in gameData)
            {
                write("Game " + gameData.IndexOf(gd) + ":");
                write(" Winner: " + gd.winner);
                write(" Game duration: " + gd.gameTimeSeconds + "s");
                write(" Number of turns: " + gd.turnCount);
            }
        }
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
