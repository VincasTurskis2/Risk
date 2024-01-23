
public interface IGameMasterPlayerView
{
    public bool is2PlayerGame{get;}
    public Player[] Players();
    public IMapPlayerView GetMap();
    public int currentPlayerNo();
    public TurnStage turnStage();
    public UIManager uiManager {get;}
    public bool allTerritoriesClaimed {get;}
    public int cardSetRewardStage();
    public Player CurrentPlayer();
    //
}