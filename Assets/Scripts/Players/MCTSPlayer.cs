using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// An implementation of a player using Monte Carlo tree search.
public class MCTSPlayer : Player
{
    
    public MCTSPlayer(GameMaster state, PlayerData data) : base(state, data)
    {
    }
    public override void StartTurn()
    {
        Debug.Log(_data.playerName + " starting turn");
        _isMyTurn = true;
        new UpdatePlaceableTroops(this, _gameMaster).execute();
        // TODO: Add rule-based troop deployment

        new EndTurnStage(this, _gameMaster).execute();
        //GameStateTreeNode root = new GameStateTreeNode(_gameState)

    }
    
    public override void AddCardsToHand(List<TerritoryCard> cards)
    {
        if(cards == null) return;
        _hand = Enumerable.Concat(_hand, cards).ToList();
    }
}
