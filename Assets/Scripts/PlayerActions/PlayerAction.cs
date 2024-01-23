public abstract class PlayerAction{

    public readonly Player caller;
    public readonly GameMaster gameMaster;

    public PlayerAction(Player Caller, IGameMasterPlayerView GameMaster)
    {
        caller = Caller;
        gameMaster = (GameMaster) GameMaster;
    }
    public abstract bool execute();
}