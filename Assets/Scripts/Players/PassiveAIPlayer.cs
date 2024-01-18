using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// An "AI" player class which does nothing. On turn start, immediately ends its turn.
public class PassiveAIPlayer : Player
{
    
    public override void Setup(GameMaster state, PlayerData data)
    {
        _gameState = state;
        if(_gameState.is2PlayerGame)
        {
            _placeableTroops = 26;
        }
        else if(_gameState.Players().Length >= 3 && _gameState.Players().Length <= 6)
        {
            _placeableTroops = 40 - ((_gameState.Players().Length - 2) * 5);
        }
        else
        {
            Debug.Log("Error in player count: there are " + _gameState.Players().Length + " players, should be between 2 and 6");
            return;
        }
        _actions = (PlayerActions) FindAnyObjectByType(typeof(PlayerActions));
        _ownedTerritories = new HashSet<ITerritoryPlayerView>();
        _hand = new List<TerritoryCard>();
        _data = data;
    }
    public override void StartTurn()
    {
        Debug.Log(_data.playerName + " starting turn");
        _isMyTurn = true;
        _placeableTroops = _actions.CalculatePlaceableTroops(this);
        if(_gameState.turnStage() == TurnStage.Setup)
        {
            List<ITerritoryPlayerView> possibleTerritories = new List<ITerritoryPlayerView>(_ownedTerritories);
            if(!_gameState.allTerritoriesClaimed)
            {
                possibleTerritories = new List<ITerritoryPlayerView>();
                foreach(ITerritoryPlayerView t in _gameState.GetMap().GetTerritories())
                {
                    
                    if(t.GetOwner() == null)
                    {
                        possibleTerritories.Add(t);
                    }
                }
            }
            int randomTerritoryNumber = Random.Range(0, possibleTerritories.Count);
            bool success = _actions.SetupDeploy(possibleTerritories[randomTerritoryNumber], this);
        }
        else if(_gameState.turnStage() == TurnStage.Deploy)
        {
            int randomTerritoryNumber = Random.Range(0, _ownedTerritories.Count);
            List<ITerritoryPlayerView> territoryList = _ownedTerritories.ToList();
            ITerritoryPlayerView territoryToDeploy = territoryList[randomTerritoryNumber];
            ITerritoryPlayerView[] territoryToDeployArray = {territoryToDeploy};
            int[] amounts = { _placeableTroops };
            _actions.DeployMultiple(territoryToDeployArray, amounts);
            EndTurn();
        }
        else
        {
            EndTurn();
        }
    }
    
    public override void DiscardCards(TerritoryCard[] cardsToDiscard)
    {
        _actions.DiscardCards(cardsToDiscard, this);
    }
    public override void AddCardsToHand(List<TerritoryCard> cards)
    {
        if(cards == null) return;
        _hand = Enumerable.Concat(_hand, cards).ToList();
    }
}
