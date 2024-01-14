using System.Collections.Generic;

public class GameStateTreeNode
{
    public GameState value {get; private set;}
    public LinkedList<GameStateTreeNode> children {get; private set;}

    public GameStateTreeNode(GameState Value)
    {
        value = Value;
        children = new LinkedList<GameStateTreeNode>();
    }
}