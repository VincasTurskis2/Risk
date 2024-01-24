using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// An "AI" player class which does nothing. On turn start, immediately ends its turn.
public class PassiveAIPlayer : Player
{
    
    public PassiveAIPlayer(GameState state, PlayerData data, bool is2PlayerGame) : base(state, data, is2PlayerGame)
    {
    }
    public PassiveAIPlayer(PassiveAIPlayer oldPlayer, GameState newState) : base(oldPlayer, newState)
    {
    }
    public override void StartTurn()
    {
        Debug.Log(_data.playerName + " starting turn");
        _isMyTurn = true;
        new UpdatePlaceableTroops(this).execute();
        if(_gameState.turnStage == TurnStage.InitDeploy || _gameState.turnStage == TurnStage.InitReinforce)
        {
            List<ITerritoryPlayerView> possibleTerritories = _gameState.Map().GetOwnedTerritories(this).ToList();
            if(_gameState.turnStage == TurnStage.InitDeploy)
            {
                possibleTerritories = new List<ITerritoryPlayerView>();
                foreach(ITerritoryPlayerView t in _gameState.Map().GetTerritories())
                {
                    if(t.GetOwner() == null)
                    {
                        possibleTerritories.Add(t);
                    }
                }
            }
            int randomTerritoryNumber = Random.Range(0, possibleTerritories.Count);
            bool success = new SetupDeploy(this, possibleTerritories[randomTerritoryNumber]).execute();
        }
        else if(_gameState.turnStage == TurnStage.Deploy)
        {
            _gameState.Map().GetOwnedTerritories(this).Count();
            int randomTerritoryNumber = Random.Range(0, _gameState.Map().GetOwnedTerritories(this).Length);
            List<ITerritoryPlayerView> territoryList = _gameState.Map().GetOwnedTerritories(this).ToList();
            ITerritoryPlayerView territoryToDeploy = territoryList[randomTerritoryNumber];
            ITerritoryPlayerView[] territoryToDeployArray = {territoryToDeploy};
            int[] amounts = { _placeableTroops };
            new DeployMultiple(this, territoryToDeployArray, amounts);
            EndTurn();
        }
        else
        {
            EndTurn();
        }
    }
    public override void AddCardsToHand(List<TerritoryCard> cards)
    {
        if(cards == null) return;
        _hand = Enumerable.Concat(_hand, cards).ToList();
    }
}
