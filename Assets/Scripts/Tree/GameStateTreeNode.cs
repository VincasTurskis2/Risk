using System.Collections.Generic;

public class GameStateTreeNode
{
    public GameState state {get; private set;}
    public GameStateTreeNode parent {get; private set;}
    public List<GameStateTreeNode> children {get; private set;}

    public float cumulativeHeuristicValue;
    public int numberOfPlaythoughs;

    public GameStateTreeNode(GameState Value, GameStateTreeNode Parent)
    {
        state = Value;
        children = new List<GameStateTreeNode>();
        numberOfPlaythoughs = 0;
        cumulativeHeuristicValue = 0;
        parent = Parent;
    }
    public GameStateTreeNode(IGameStatePlayerView Value, GameStateTreeNode Parent)
    {
        state = (GameState) Value;
        children = new List<GameStateTreeNode>();
        numberOfPlaythoughs = 0;
        cumulativeHeuristicValue = 0;
        parent = Parent;
    }

    public void AddChild(GameState child)
    {
        children.Add(new GameStateTreeNode(child, this));
    }
}