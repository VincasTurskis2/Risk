using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditorInternal;
using System;

// An implementation of a player using Monte Carlo tree search.
public class MCTSPlayer : Player
{
    public float timeForSearch = 1f;
    public double C = Math.Sqrt(2);
    public MCTSPlayer(GameState state, PlayerData data, bool is2PlayerGame) : base(state, data, is2PlayerGame)
    {
    }
    public MCTSPlayer(MCTSPlayer oldPlayer, GameState newState) : base(oldPlayer, newState)
    {
    }
    public override void StartTurn()
    {
        Debug.Log(_data.playerName + " starting turn");
        _isMyTurn = true;
        new UpdatePlaceableTroops(this).execute();
        // TODO: Add rule-based troop deployment

        new EndTurnStage(this).execute();
        

    }
    
    public override void AddCardsToHand(List<TerritoryCard> cards)
    {
        if(cards == null) return;
        _hand = Enumerable.Concat(_hand, cards).ToList();
    }

    public List<PlayerAction> PerformMTCS()
    {
        List<PlayerAction> result = new();
        GameStateTreeNode root = new GameStateTreeNode(_gameState, null);
        float curTime = 0f;
        while (curTime < timeForSearch)
        {
        //     State leaf = select(tree)
        //     State child = expand(leaf)
        //     tree.addNode(child, leaf)
        //     result = simulate(child)
        //     back_propagate(result, child)
        }
        
        
        //return {Action that has the highest number of playouts}
        return result;
    }

    public GameStateTreeNode Select(GameStateTreeNode curNode)
    {
        if(curNode.numberOfPlaythoughs == 0) return curNode;

        return curNode;
    }



    public float heuristicEvaluation(GameState state, Player player)
    {
        return troopSharePercent(state, player);
    }

    public float troopSharePercent(GameState state, Player player)
    {
        int totalOwnTroops = 0, totalOtherTroops = 0;
        for(int i = 0; i < state.map.Territories.Length; i++)
        {
            if(state.map.Territories[i].Owner.Equals(player.GetData().playerName))
            {
                totalOwnTroops += state.map.Territories[i].TroopCount;
            }
            else
            {
                totalOtherTroops += state.map.Territories[i].TroopCount;
            }
        };
        if(totalOwnTroops + totalOtherTroops == 0) return 0;
        
        return totalOwnTroops/(totalOwnTroops + totalOtherTroops);
        
    }
    public float UCB1(GameStateTreeNode node)
    {
        double result = 0;
        if(node.numberOfPlaythoughs != 0)
        {
            result = node.cumulativeHeuristicValue / node.numberOfPlaythoughs;
        }
        if(node.parent != null)
        {
            result += C * Math.Sqrt(Math.Log(node.parent.numberOfPlaythoughs)/node.numberOfPlaythoughs);
        }
        return (float)result;
    }
}
