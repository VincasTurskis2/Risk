public interface IGameStatePlayerView
{
    public IOtherPlayer[] Players();
    public IMapPlayerView Map();
    public int currentPlayerNo {get;}
    public TurnStage turnStage {get;}
    public int cardSetRewardStage {get;}
    public IOtherPlayer getPlayerViewFromName(string playerName);
}