using System.Collections.Generic;

public class GameStateTreeNode
{
    public GameState state {get; private set;}
    public List<GameStateTreeNode> children {get; private set;}

    public float heuristicValue;
    public int numberOfPlaythoughs;

    public GameStateTreeNode(GameState Value)
    {
        state = Value;
        children = new List<GameStateTreeNode>();
        numberOfPlaythoughs = 0;
        heuristicValue = 0;
    }
    public GameStateTreeNode(IGameStatePlayerView Value)
    {
        state = (GameState) Value;
        children = new List<GameStateTreeNode>();
        numberOfPlaythoughs = 0;
        heuristicValue = 0;
    }

    public void AddChild(GameState child)
    {
        children.Add(new GameStateTreeNode(child));
    }
}