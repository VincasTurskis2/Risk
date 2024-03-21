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
        int attackingTroops = from.TroopCount - 1, defendingTroops = to.TroopCount;
        int attackingLosses = Math.Min(attackingTroops, defendingTroops);
        int defendingLosses;
        if (attackingTroops >= 3)
        {
            defendingLosses = (int)Math.Round(attackingLosses * 1.17176f);
        }
        else defendingLosses = attackingLosses;
        if(attackingLosses == attackingTroops && defendingLosses == defendingTroops)
        {
            from.TroopCount = 1;
            to.TroopCount = 1;
        }
        else if(attackingLosses >= attackingTroops)
        {
            to.TroopCount = Math.Max(defendingTroops - defendingLosses, 1);
            from.TroopCount = 1;
        }
        else
        {
            var loser = newState.getPlayerFromName(to.Owner);
            to.SetOwner(newState.getPlayerFromName(from.Owner), true);
            to.TroopCount = Math.Max(1, attackingTroops - attackingLosses);
            from.TroopCount = 1;
            if (newState.map.GetOwnedTerritories(loser).Count() == 0)
            {
                //GameMaster.Instance.OnPlayerLoss(loser, newState);
            }
        }
        return AddChild(newState, attack);
    }

    public GameStateTreeNode Expand(Player player)
    {
        
        if (fullyExpanded) // || state.terminalState == true
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