public abstract class PlayerAction{

    public readonly Player caller;

    public PlayerAction(Player Caller)
    {
        caller = Caller;
    }
    public abstract bool execute();
}