using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// An "AI" player class which does nothing. On turn start, immediately ends its turn.
public class PassiveAIPlayer : Player
{
    
    public PassiveAIPlayer(GameMaster state, PlayerData data) : base(state, data)
    {
    }
    public override void StartTurn()
    {
        Debug.Log(_data.playerName + " starting turn");
        _isMyTurn = true;
        new UpdatePlaceableTroops(this, _gameMaster).execute();
        if(_gameMaster.turnStage() == TurnStage.Setup)
        {
            List<ITerritoryPlayerView> possibleTerritories = _gameMaster.GetMap().GetOwnedTerritories(this).ToList();
            if(!_gameMaster.allTerritoriesClaimed)
            {
                possibleTerritories = new List<ITerritoryPlayerView>();
                foreach(ITerritoryPlayerView t in _gameMaster.GetMap().GetTerritories())
                {
                    if(t.GetOwner() == null)
                    {
                        possibleTerritories.Add(t);
                    }
                }
            }
            int randomTerritoryNumber = Random.Range(0, possibleTerritories.Count);
            bool success = new SetupDeploy(this, _gameMaster, possibleTerritories[randomTerritoryNumber]).execute();
        }
        else if(_gameMaster.turnStage() == TurnStage.Deploy)
        {
            _gameMaster.GetMap().GetOwnedTerritories(this).Count();
            int randomTerritoryNumber = Random.Range(0, _gameMaster.GetMap().GetOwnedTerritories(this).Length);
            List<ITerritoryPlayerView> territoryList = _gameMaster.GetMap().GetOwnedTerritories(this).ToList();
            ITerritoryPlayerView territoryToDeploy = territoryList[randomTerritoryNumber];
            ITerritoryPlayerView[] territoryToDeployArray = {territoryToDeploy};
            int[] amounts = { _placeableTroops };
            new DeployMultiple(this, _gameMaster, territoryToDeployArray, amounts);
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
