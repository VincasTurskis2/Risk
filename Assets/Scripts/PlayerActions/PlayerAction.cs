public abstract class PlayerAction{

    public readonly Player caller;
    public readonly GameMaster gameMaster;

    public PlayerAction(Player Caller, GameMaster Master)
    {
        caller = Caller;
        gameMaster = Master;
    }
    public abstract bool execute();
}