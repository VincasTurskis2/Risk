using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    public List<Attack> getAllPossibleAttacks()
    {
        List<Attack> result = new();
        ITerritoryPlayerView[] curPlayerTerritories = state.map.GetOwnedTerritories(state.players[state.currentPlayerNo]);
        for(int i = 0; i < curPlayerTerritories.Length; i++)
        {
            TerritoryData curTerritory = (TerritoryData) curPlayerTerritories[i];
            if(curTerritory.TroopCount == 1) continue;
            TerritoryData[] unownedNeighbors = state.map.GetRawTerritories(curTerritory.Neighbors).Where(x => !x.Owner.Equals(curTerritory.Owner)).ToArray();
            foreach(var neighbor in unownedNeighbors)
            {
                result.Add(new Attack(state.getPlayerFromName(curTerritory.Owner), curTerritory, neighbor));
            }
        }
        return result;
    }
}