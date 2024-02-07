using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A class that keeps track of game state elements
public class GameMaster : MonoBehaviour
{

    
    public static GameMaster Instance {get; private set;}
    void Awake()
    {
        Instance = this;
    }
    private GameMaster()
    {
        
    }
    public GameState state;

    public static readonly int[] ContinentValues = {2, 3, 7, 5, 5, 2};
    public static readonly int[] CardSetRewards = {4, 6, 8, 10, 12, 15};
    public static int[] ContinentCount = {0, 0, 0, 0, 0, 0};

    public bool is2PlayerGame {get; private set;}

    public float gameTimeElapsedSeconds {get; private set;} = 0;

    public int turnCount {get; private set;} = 1;


    public void Setup(PlayerData[] playerData)
    {
        state = new GameState();
        is2PlayerGame = false;
        Setupplayers(playerData);
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
        Setupplayers(players);
        SetupTerritories();
        state.cardDeck.Setup(state.map.Territories);

    }
    public void Setupplayers(PlayerData[] players)
    {
        GameObject playerObject;
        if(players.Length == 2)
        {
            is2PlayerGame = true;
            state.players = new Player[players.Length + 1];
            state.players[players.Length] = new NeutralArmyPlayer(state);
        }
        else
        {
            state.players = new Player[players.Length];
        }
        for(int i = 0; i < players.Length; i++)
        {
            switch(players[i].playerType){
                case PlayerType.Human:
                    HumanPlayer hp = new HumanPlayer(state, players[i], is2PlayerGame);
                    playerObject = (GameObject)Resources.Load("prefabs/HumanPlayer", typeof(GameObject));
                    Instantiate(playerObject, new Vector3(), Quaternion.identity).GetComponent<HumanPlayerWrapper>().Setup(hp);
                    state.players[i] = hp;
                    break;
                case PlayerType.PassiveAI:
                    state.players[i] = new PassiveAIPlayer(state, players[i], is2PlayerGame);
                    break;
                case PlayerType.Neutral:
                    state.players[i] = new NeutralArmyPlayer(state);
                    break;
                case PlayerType.MCTS:
                    state.players[i] = new MCTSPlayer(state, players[i], is2PlayerGame);
                    break;
                default:
                    break;
            }
        }
        state.currentPlayerNo = 0;
        state.turnStage = TurnStage.InitDeploy;
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
                    state.map.Territories[i].SetOwner(state.players[2], false);
                    player3Rem -= 1;
                }
                else if(rand - player3Rem- player2Rem < 0)
                {
                    state.map.Territories[i].SetOwner(state.players[1], false);
                    player2Rem -= 1;
                }
                else
                {
                    state.map.Territories[i].SetOwner(state.players[0], false);
                    player1Rem -= 1;
                }
                state.map.Territories[i].TroopCount = 1;
                ContinentCount[(int) state.map.Territories[i].Continent]++;
            }
            state.turnStage = TurnStage.InitReinforce;
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
        if(state.currentPlayerNo >= state.players.Length)
        {
            state.currentPlayerNo = 0;
            turnCount++;
        }
        if(state.turnStage == TurnStage.InitDeploy)
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
                state.turnStage = TurnStage.InitReinforce;
            }
        }
        else if(state.turnStage != TurnStage.InitReinforce)
        {
            state.turnStage = TurnStage.Deploy;
        }
        state.players[state.currentPlayerNo].StartTurn();
    }

    public void OnPlayerLoss(Player player)
    {
        if(state.map.GetOwnedTerritories(player).Length != 0) return;
        Debug.Log(player.GetData().playerName + " has lost!");
        if(player.IsMyTurn())
        {
            player.EndTurn();
        }
        List<Player> newplayers = new();
        int loserNo = 0;
        for(int i = 0; i < state.players.Length; i++)
        {
            if(state.players[i] != player)
            {
                newplayers.Add(state.players[i]);
            }
            else
            {
                loserNo = i;
            }
        }
        state.players = newplayers.ToArray();
        if(state.currentPlayerNo >= loserNo)
        {
            state.currentPlayerNo--;
        }
        if(player is NeutralArmyPlayer)
        {
            return;
        }
        if(state.players.Length == 1 || (state.players.Length == 2 && is2PlayerGame))
        {
            Debug.Log(state.players[0].GetData().playerName + " Has won!");
            
            {
                UIManager.Instance.DisplayVictoryPanel(state.players[0].GetData().playerName, turnCount, gameTimeElapsedSeconds);
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
        return state.players[state.currentPlayerNo];
    }

    public void EndSetupStage()
    {
        if(state.turnStage != TurnStage.InitReinforce && state.turnStage != TurnStage.InitDeploy) return;
        state.turnStage = TurnStage.Deploy;
        turnCount = 1;
    }
    void Update()
    {
        gameTimeElapsedSeconds += Time.deltaTime;
    }
}
