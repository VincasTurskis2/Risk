using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditorInternal;

// An implementation of a player using Monte Carlo tree search.
public class MCTSPlayer : Player
{
    public float timeForSearch = 1f;
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
        GameStateTreeNode root = new GameStateTreeNode(_gameState);
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

    public IGameStatePlayerView Select(GameStateTreeNode tree)
    {
        return tree.state;
    }
}
