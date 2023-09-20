using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A class that keeps track of game state elements
public class GameState : MonoBehaviour, IGameStatePlayerView
{
    [field: SerializeField]
    public CardDeck cardDeck {get; set;}
    [field: SerializeField]
    public Player[] Players {get; set;}
    [field: SerializeField]
    public Territory[] territories {get; set;}

    [field: SerializeField]
    public int currentPlayerNo{get; private set;}
    [field: SerializeField]
    public TurnStage turnStage {get; set;}
    public UIManager uiManager {get; set;}

    public static readonly int[] ContinentValues = {2, 3, 7, 5, 5, 2};
    public static int[] ContinentCount = {0, 0, 0, 0, 0, 0};

    public bool allTerritoriesClaimed {get; set;} = false;

    public int cardSetRewardStage {get; set;} = 0;

    public bool is2PlayerGame {get; set;}

    public float gameTimeElapsedSeconds {get; private set;} = 0;

    public int turnCount {get; private set;} = 1;


    public void Setup(PlayerData[] playerData)
    {
        is2PlayerGame = false;
        uiManager = gameObject.GetComponent<UIManager>();
        cardDeck = gameObject.GetComponent<CardDeck>();
        SetupPlayers(playerData);
        SetupTerritories();
        cardDeck.SetupDeck();
    }
    public void Setup()
    {
        is2PlayerGame = false;
        PlayerData[] players = new PlayerData[2];
        players[0] = new PlayerData("Vince", PlayerType.Human, ColorPreset.Green);
        players[1] = new PlayerData("JohnGPT", PlayerType.PassiveAI, ColorPreset.Pink);
        uiManager = gameObject.GetComponent<UIManager>();
        cardDeck = gameObject.GetComponent<CardDeck>();
        SetupPlayers(players);
        SetupTerritories();
        cardDeck.SetupDeck();
    }
    public void SetupPlayers(PlayerData[] players)
    {
        GameObject playerObject, playerObjectInstance;
        if(players.Length == 2)
        {
            is2PlayerGame = true;
            Players = new Player[players.Length + 1];
            playerObject = (GameObject)Resources.Load("prefabs/NeutralArmyPlayer", typeof(GameObject));
            playerObjectInstance = Instantiate(playerObject, new Vector3(), Quaternion.identity);
            Players[players.Length] = playerObjectInstance.GetComponent<NeutralArmyPlayer>();
            Players[players.Length].Setup(this, null);
        }
        else
        {
            Players = new Player[players.Length];
        }
        for(int i = 0; i < players.Length; i++)
        {
            switch(players[i].playerType){
                case PlayerType.Human:
                    playerObject = (GameObject)Resources.Load("prefabs/HumanPlayer", typeof(GameObject));
                    playerObjectInstance = Instantiate(playerObject, new Vector3(), Quaternion.identity);
                    Players[i] = playerObjectInstance.GetComponent<HumanPlayer>();
                    break;
                case PlayerType.PassiveAI:
                    playerObject = (GameObject)Resources.Load("prefabs/PassiveAIPlayer", typeof(GameObject));
                    playerObjectInstance = Instantiate(playerObject, new Vector3(), Quaternion.identity);
                    Players[i] = playerObjectInstance.GetComponent<PassiveAIPlayer>();
                    break;
                default:
                    break;
            }
            Players[i].Setup(this, players[i]);
        }
        currentPlayerNo = 0;
        turnStage = TurnStage.Setup;
    }

    public void SetupTerritories()
    {
        GameObject[] territoryObjects = GameObject.FindGameObjectsWithTag("Territory");
        territories = new Territory[territoryObjects.Length];
        if(is2PlayerGame)
        {
            int player1Rem = 14, player2Rem = 14, player3Rem = 14;
            for(int i = 0; i < territories.Length; i++)
            {
                territories[i] = territoryObjects[i].GetComponent<Territory>();
                territories[i].Setup();

                int rand = Random.Range(1, player1Rem + player2Rem + player3Rem + 1);
                if(rand - player3Rem <= 0)
                {
                    territories[i].SetOwner(Players[2]);
                    player3Rem--;
                }
                else if(rand - player3Rem - player2Rem < 0)
                {
                    territories[i].SetOwner(Players[1]);
                    player2Rem--;
                }
                else
                {
                    territories[i].SetOwner(Players[0]);
                    player1Rem--;
                }
                territories[i].TroopCount = 1;
                ContinentCount[(int) territories[i].Continent]++;
            }
            allTerritoriesClaimed = true;
        }
        else
        {
            for(int i = 0; i < territories.Length; i++)
            {
                territories[i] = territoryObjects[i].GetComponent<Territory>();
                territories[i].Setup();

                territories[i].SetOwner(null);
                territories[i].TroopCount = 0;

                ContinentCount[(int) territories[i].Continent]++;
            }
        }   
    }

    // Function called by the player scripts when they end their turn.
    public void EndTurn()
    {
        currentPlayerNo++;
        if(currentPlayerNo >= Players.Length)
        {
            currentPlayerNo = 0;
            turnCount++;
        }
        if(turnStage == TurnStage.Setup)
        {
            if(!allTerritoriesClaimed)
            {
                bool endOfClaimingPhase = true;
                foreach(Territory t in territories)
                {
                    if(t.Owner == null)
                    {
                        endOfClaimingPhase = false;
                        break;
                    }
                }
                if(endOfClaimingPhase)
                {
                    allTerritoriesClaimed = true;
                }
            }
        }
        else
        {
            turnStage = TurnStage.Deploy;
        }
        Players[currentPlayerNo].StartTurn();
    }

    public void OnPlayerLoss(Player player)
    {
        if(player.GetOwnedTerritories().Count != 0) return;
        Debug.Log(player.GetData().playerName + " has lost!");
        if(player.IsMyTurn())
        {
            player.EndTurn();
        }
        List<Player> newPlayers = new();
        int loserNo = 0;
        for(int i = 0; i < Players.Length; i++)
        {
            if(Players[i] != player)
            {
                newPlayers.Add(Players[i]);
            }
            else
            {
                loserNo = i;
            }
        }
        Players = newPlayers.ToArray();
        if(currentPlayerNo >= loserNo)
        {
            currentPlayerNo--;
        }
        if(Players.Length == 1 || (Players.Length == 2 && is2PlayerGame))
        {
            Debug.Log(Players[0].GetData().playerName + " Has won!");
            uiManager.DisplayVictoryPanel(Players[0].GetData().playerName, turnCount, gameTimeElapsedSeconds);
        }
    }
    
    // This says "0 references", but it's used by the UI button to end stage! Don't delete!
    public void EndTurnStage()
    {
        CurrentPlayer().EndTurnStage();
    }
    public Player CurrentPlayer()
    {
        return Players[currentPlayerNo];
    }

    public void EndSetupStage()
    {
        if(turnStage != TurnStage.Setup) return;
        turnStage = TurnStage.Deploy;
        turnCount = 1;
    }

    void Update()
    {
        gameTimeElapsedSeconds += Time.deltaTime;
    }
}
