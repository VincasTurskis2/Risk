using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameStateTreeNode
{
    public GameState state {get; private set;}
    public GameStateTreeNode parent {get; private set;}
    public Attack sourceMove;
    public List<GameStateTreeNode> children {get; private set;}
    public bool fullyExpanded = false;

    public float cumulativeHeuristicValue;
    public int numberOfPlaythoughs;

    public GameStateTreeNode(GameState Value, GameStateTreeNode Parent, Attack SourceMove)
    {
        state = Value;
        children = new List<GameStateTreeNode>();
        numberOfPlaythoughs = 0;
        cumulativeHeuristicValue = 0;
        parent = Parent;
        sourceMove = SourceMove;
    }
    public GameStateTreeNode(IGameStatePlayerView Value, GameStateTreeNode Parent, Attack SourceMove)
    {
        state = (GameState) Value;
        children = new List<GameStateTreeNode>();
        numberOfPlaythoughs = 0;
        cumulativeHeuristicValue = 0;
        parent = Parent;
        sourceMove = SourceMove;
    }

    public GameStateTreeNode AddChild(GameState child, Attack SourceMove)
    {
        var newNode = new GameStateTreeNode(child, this, SourceMove);
        children.Add(newNode);
        return newNode;
    }

    public GameStateTreeNode Apply(Attack attack)
    {
        //TODO: account for multiple outcomes due to dice rolls
        GameState newState = new GameState(state);

        if(attack == null)
        {
            newState.turnStage = TurnStage.Reinforce;
            return AddChild(newState, attack);
        }
        var from = newState.map.GetRawTerritory(attack.IFrom.TerritoryName);
        var to = newState.map.GetRawTerritory(attack.ITo.TerritoryName);
        if(from.TroopCount <= to.TroopCount)
        {
            to.TroopCount = to.TroopCount - from.TroopCount + 1;
            from.TroopCount = 1;
        }
        else if(from.TroopCount == to.TroopCount + 1)
        {
            to.SetOwner(newState.getPlayerFromName(from.Owner), true);
            to.TroopCount = 1;
            from.TroopCount = 1;
        }
        else
        {
            to.SetOwner(newState.getPlayerFromName(from.Owner), true);
            to.TroopCount = from.TroopCount - to.TroopCount - 1;
            from.TroopCount = 1;
        }
        return AddChild(newState, attack);
    }

    public GameStateTreeNode Expand(Player player)
    {
        if(fullyExpanded)
        {
            return this;
        }
        var possibleAttacks = state.getAllPossibleAttacks(player);
        possibleAttacks.Add(null);
        var attacks = new List<Attack>(possibleAttacks);
        foreach(var child in children)
        {
            foreach(var attack in possibleAttacks)
            {       
                if(child.sourceMove == null)
                {
                    if(attack == null)
                    {
                        attacks.Remove(null);
                    }
                }
                else if(child.sourceMove.Equals(attack))
                {
                    attacks.Remove(attack);
                }
            }
        }
        if(attacks.Count() == 1)
        {
            fullyExpanded = true;
        }
        var randAttack = attacks[new Random().Next(0, attacks.Count())];
        return Apply(randAttack);
    }
}