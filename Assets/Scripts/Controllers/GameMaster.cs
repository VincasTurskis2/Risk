using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A class that keeps track of game state elements
public class GameMaster : MonoBehaviour, IGameMasterPlayerView
{
    public GameState state;
    public UIManager uiManager {get; set;}

    public static readonly int[] ContinentValues = {2, 3, 7, 5, 5, 2};
    public static readonly int[] CardSetRewards = {4, 6, 8, 10, 12, 15};
    public static int[] ContinentCount = {0, 0, 0, 0, 0, 0};

    public bool allTerritoriesClaimed {get; set;} = false;

    public bool is2PlayerGame {get; private set;}

    public float gameTimeElapsedSeconds {get; private set;} = 0;

    public int turnCount {get; private set;} = 1;


    public void Setup(PlayerData[] playerData)
    {
        state = new GameState();
        is2PlayerGame = false;
        uiManager = gameObject.GetComponent<UIManager>();
        SetupPlayers(playerData);
        SetupTerritories();
        state.cardDeck.Setup(state.map.Territories);
    }
    public void Setup()
    {
        state = new GameState();
        is2PlayerGame = false;
        PlayerData[] players = new PlayerData[2];
        players[0] = new PlayerData("Vince", PlayerType.Human, ColorPreset.Green);
        players[1] = new PlayerData("JohnGPT", PlayerType.PassiveAI, ColorPreset.Pink);
        uiManager = gameObject.GetComponent<UIManager>();
        SetupPlayers(players);
        SetupTerritories();
        state.cardDeck.Setup(state.map.Territories);

    }
    public void SetupPlayers(PlayerData[] players)
    {
        GameObject playerObject;
        if(players.Length == 2)
        {
            is2PlayerGame = true;
            state.Players = new Player[players.Length + 1];
            state.Players[players.Length] = new NeutralArmyPlayer(this);
        }
        else
        {
            state.Players = new Player[players.Length];
        }
        for(int i = 0; i < players.Length; i++)
        {
            switch(players[i].playerType){
                case PlayerType.Human:
                    HumanPlayer hp = new HumanPlayer(this, players[i]);
                    playerObject = (GameObject)Resources.Load("prefabs/HumanPlayer", typeof(GameObject));
                    Instantiate(playerObject, new Vector3(), Quaternion.identity).GetComponent<HumanPlayerWrapper>().Setup(hp);
                    state.Players[i] = hp;
                    break;
                case PlayerType.PassiveAI:
                    state.Players[i] = new PassiveAIPlayer(this, players[i]);
                    break;
                case PlayerType.Neutral:
                    state.Players[i] = new NeutralArmyPlayer(this);
                    break;
                case PlayerType.MCTS:
                    state.Players[i] = new MCTSPlayer(this, players[i]);
                    break;
                default:
                    break;
            }
        }
        state.currentPlayerNo = 0;
        state.turnStage = TurnStage.Setup;
    }

    public void SetupTerritories()
    {
        GameObject[] territoryObjects = GameObject.FindGameObjectsWithTag("Territory");
        Territory[] territoryWrappers = new Territory[territoryObjects.Length];
        for(int i = 0; i < territoryObjects.Length; i++)
        {
            territoryWrappers[i] = territoryObjects[i].GetComponent<Territory>();
        }
        state.map = new Map(territoryWrappers);
        if(is2PlayerGame)
        {
            int player1Rem = 14, player2Rem = 14, player3Rem = 14;
            for(int i = 0; i < state.map.Territories.Length; i++)
            {
                int rand = Random.Range(0, player1Rem + player2Rem + player3Rem + 1);
                if(rand - player3Rem < 0)
                {
                    state.map.Territories[i].SetOwner(state.Players[2], false);
                    player3Rem -= 1;
                }
                else if(rand - player3Rem- player2Rem < 0)
                {
                    state.map.Territories[i].SetOwner(state.Players[1], false);
                    player2Rem -= 1;
                }
                else
                {
                    state.map.Territories[i].SetOwner(state.Players[0], false);
                    player1Rem -= 1;
                }
                state.map.Territories[i].TroopCount = 1;
                ContinentCount[(int) state.map.Territories[i].Continent]++;
            }
            allTerritoriesClaimed = true;
        }
        else
        {
            for(int i = 0; i < state.map.Territories.Length; i++)
            {
                state.map.Territories[i].SetOwner(null, false);
                state.map.Territories[i].TroopCount = 0;

                ContinentCount[(int) state.map.Territories[i].Continent]++;
            }
        }
    }

    // Function called by the player scripts when they end their turn.
    public void EndTurn()
    {
        state.currentPlayerNo++;
        if(state.currentPlayerNo >= state.Players.Length)
        {
            state.currentPlayerNo = 0;
            turnCount++;
        }
        if(state.turnStage == TurnStage.Setup)
        {
            if(!allTerritoriesClaimed)
            {
                bool endOfClaimingPhase = true;
                foreach(TerritoryData t in state.map.Territories)
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
            state.turnStage = TurnStage.Deploy;
        }
        state.Players[state.currentPlayerNo].StartTurn();
    }

    public void OnPlayerLoss(Player player)
    {
        if(state.map.GetOwnedTerritories(player).Length != 0) return;
        Debug.Log(player.GetData().playerName + " has lost!");
        if(player.IsMyTurn())
        {
            player.EndTurn();
        }
        List<Player> newPlayers = new();
        int loserNo = 0;
        for(int i = 0; i < state.Players.Length; i++)
        {
            if(state.Players[i] != player)
            {
                newPlayers.Add(state.Players[i]);
            }
            else
            {
                loserNo = i;
            }
        }
        state.Players = newPlayers.ToArray();
        if(state.currentPlayerNo >= loserNo)
        {
            state.currentPlayerNo--;
        }
        if(player is NeutralArmyPlayer)
        {
            return;
        }
        if(state.Players.Length == 1 || (state.Players.Length == 2 && is2PlayerGame))
        {
            Debug.Log(state.Players[0].GetData().playerName + " Has won!");
            
            {
                uiManager.DisplayVictoryPanel(state.Players[0].GetData().playerName, turnCount, gameTimeElapsedSeconds);
            }
        }
    }
    
    // This says "0 references", but it's used by the UI button to end stage! Don't delete!
    public void EndTurnStage()
    {
        CurrentPlayer().EndTurnStage();
    }
    public Player CurrentPlayer()
    {
        return state.Players[state.currentPlayerNo];
    }

    public void EndSetupStage()
    {
        if(state.turnStage != TurnStage.Setup) return;
        state.turnStage = TurnStage.Deploy;
        turnCount = 1;
    }
    void Update()
    {
        gameTimeElapsedSeconds += Time.deltaTime;
    }

    public CardDeck cardDeck()
    {
        return state.cardDeck;
    }
    public Player[] Players()
    {
        return state.Players;
    }
    public TerritoryData[] territories()
    {
        return state.map.Territories;
    }
    public int currentPlayerNo()
    {
        return state.currentPlayerNo;
    }
    public TurnStage turnStage()
    {
        return state.turnStage;
    }
    public int cardSetRewardStage()
    {
        return state.cardSetRewardStage;
    }

    public IMapPlayerView GetMap()
    {
        return state.map;
    }

    public void HighlightTerritory(ITerritoryPlayerView territory, bool toHighlight)
    {
        if(territory == null) return;
        TerritoryData Territory = (TerritoryData) territory;
        if(toHighlight)
        {
            Territory.territoryColor = Helpers.GetHighlighedColorVersion(getPlayerFromName(Territory.Owner).GetData().playerColor);
        }
        else
        {
            Territory.territoryColor = getPlayerFromName(Territory.Owner).GetData().playerColor;
        }
    }

    public Player getPlayerFromName(string playerName)
    {
        for(int i = 0; i < state.Players.Length; i++)
        {
            if(playerName.Equals(state.Players[i].GetData().playerName))
            {
                return state.Players[i];
            }
        }
        return null;
    }
}
