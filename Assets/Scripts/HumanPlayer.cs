using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HumanPlayer : MonoBehaviour, Player
{
    private UIManager _uiManager;
    [SerializeField]
    private PlayerData _data;
    [SerializeField]
    private int _placeableTroops;
    [SerializeField]
    private HashSet<Territory> _ownedTerritories;
    private GameState _gameState;

    [SerializeField]
    private bool _isMyTurn = false;
    [SerializeField]
    private Territory _selectedTerritory = null;
    private Territory _previouslySelectedTerritory = null;
    private PlayerActions _actions;

    private int _cardSetRewardStage = 0;
    private List<TerritoryCard> _hand;

    private bool _cardEligible;

    public void Setup(GameState state, PlayerData data)
    {
        _gameState = state;
        if(_gameState.Players.Length >= 2 && _gameState.Players.Length <= 6)
        {
            _placeableTroops = 40 - ((_gameState.Players.Length - 2) * 5);
        }
        else
        {
            Debug.Log("Error in player count: there are " + _gameState.Players.Length + " players, should be between 2 and 6");
            return;
        }
        _actions = (PlayerActions) FindAnyObjectByType(typeof(PlayerActions));
        _uiManager = (UIManager) FindAnyObjectByType(typeof(UIManager));
        _ownedTerritories = new HashSet<Territory>();
        _hand = new List<TerritoryCard>();
        _data = data;
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && _isMyTurn && !_uiManager.PanelOverlayIsDisplayed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 15);
            if(hit.collider != null)
            {
                bool success = false;
                SelectTerritory(hit.collider.gameObject.GetComponent<Territory>());
                switch(_gameState.turnStage){
                    case TurnStage.Setup:
                        _actions.SetupDeploy(_selectedTerritory, this);
                        break;
                    case TurnStage.Deploy:
                        if((Object)_selectedTerritory.Owner == this)
                        {
                            success = _actions.Deploy(_selectedTerritory);
                        }
                        break;
                    case TurnStage.Attack:
                        if(_selectedTerritory != null && _previouslySelectedTerritory != null)
                        {
                            if((Object)_previouslySelectedTerritory.Owner == this && (Object)_selectedTerritory.Owner != this && _selectedTerritory.IsANeighbor(_previouslySelectedTerritory) && _previouslySelectedTerritory.TroopCount > 1)
                            {
                                _uiManager.DisplayAttackPanel(_previouslySelectedTerritory, _selectedTerritory);
                                SelectTerritory(null);
                            }
                        }
                        break;
                    case TurnStage.Reinforce:
                        if(_selectedTerritory != null && _previouslySelectedTerritory != null)
                        {
                            if(_selectedTerritory.Owner == _previouslySelectedTerritory.Owner && _selectedTerritory.IsANeighbor(_previouslySelectedTerritory))
                            {
                                _uiManager.DisplayFortifyPanel(_previouslySelectedTerritory, _selectedTerritory);
                                SelectTerritory(null);
                            }
                        }
                        break;
                }
            }
            else
            {
                SelectTerritory(null);
            }
        }
    }
    // Function called when a player's turn should start
    public void StartTurn()
    {
        Debug.Log(_data.playerName + " starting turn");
        _placeableTroops = _actions.CalculatePlaceableTroops(this);
        _isMyTurn = true;
    }

    // Function called by other objects when a player's turn should end (eg. they make a non-attacking troop move)
    public void EndTurn()
    {
        Debug.Log(_data.playerName + " ending turn");
        _isMyTurn = false;
        _actions.EndTurn(this);
    }
    public void EndTurnStage()
    {
        switch(_gameState.turnStage){
            case TurnStage.Setup:
                break;
            case TurnStage.Deploy:
                if(_hand.Count >= 5) return;
                _gameState.turnStage = TurnStage.Attack;
                break;
            case TurnStage.Attack:
                _gameState.turnStage = TurnStage.Reinforce;
                break;
            case TurnStage.Reinforce:
                EndTurn();
                break;
        }
    }
    public bool IsMyTurn()
    {
        return _isMyTurn;
    }
    public HashSet<Territory> GetOwnedTerritories()
    {
        return _ownedTerritories;
    }


    public int GetPlaceableTroopNumber()
    {
        return _placeableTroops;
    }
    public void SetPlaceableTroopNumber(int newNumber)
    {
        _placeableTroops = newNumber;
    }
    public void DecrementPlaceableTroops()
    {
        _placeableTroops--;
    }

    public PlayerData GetData()
    {
        return _data;
    }

    public void IncrementCardSetReward()
    {
        _cardSetRewardStage++;
    }

    public int GetCardSetRewardStage()
    {
        return _cardSetRewardStage;
    }

    public void DiscardCards(TerritoryCard[] cardsToDiscard)
    {
        for(int i = 0; i < cardsToDiscard.Length; i++)
        {
            if(_hand.Contains(cardsToDiscard[i]))
            {
                _gameState.cardDeck.DiscardCard(cardsToDiscard[i]);
                _hand.Remove(cardsToDiscard[i]);
            }
        }
    }

    public List<TerritoryCard> GetCardHand()
    {
        return _hand;
    }
    public void AddCardsToHand(List<TerritoryCard> cards)
    {
        _hand = Enumerable.Concat(_hand, cards).ToList();
    }

    public void SetCardEligible(bool set)
    {
        _cardEligible = set;
    }
    public bool IsCardEligible()
    {
        return _cardEligible;
    }

    private void SelectTerritory(Territory tr)
    {
        if(_selectedTerritory != null)
        {
            _selectedTerritory.Highlight(false);
        }
        if(tr != null)
        {
            tr.Highlight(true);
        }
        _previouslySelectedTerritory = _selectedTerritory;
        _selectedTerritory = tr;
    }
}
