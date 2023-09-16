using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class HumanPlayer :  Player
{
    private UIManager _uiManager;
    private Territory _selectedTerritory = null;
    private Territory _previouslySelectedTerritory = null;

    public override void Setup(GameState state, PlayerData data)
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

        // Set the "trade in cards" button to call a function from this class
        // Needs to be done in script, because an object with this class attached does not exist in the scene before running it
        _uiManager.GetCardUIManager().GetTradeInCardsButton().onClick.AddListener(TradeInCards);

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


            //Check if the mouse is over a UI element
            bool isOnUI = false;
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            for(int i = 0; i < raycastResults.Count; i++)
            {
                RaycastResult curRaysastResult = raycastResults[i];
                if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("InteractableUI"))
                {
                    isOnUI = true;
                }
            }

            if(hit.collider != null && isOnUI == false)
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
    public override void StartTurn()
    {
        Debug.Log(_data.playerName + " starting turn");
        _placeableTroops = _actions.CalculatePlaceableTroops(this);
        _isMyTurn = true;
        _uiManager.RedrawCardPanel(this);
    }

    
    

    public override void DiscardCards(TerritoryCard[] cardsToDiscard)
    {
        _actions.DiscardCards(cardsToDiscard, this);
        _uiManager.RedrawCardPanel(this);
    }
    public override void AddCardsToHand(List<TerritoryCard> cards)
    {
        if(cards == null) return;
        _hand = Enumerable.Concat(_hand, cards).ToList();
        
        for(int i = 0; i < cards.Count; i++)
        {
            _uiManager.AddCardToPanel(cards[i]);
        }
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

    public void TradeInCards()
    {
        if(_gameState.turnStage != TurnStage.Deploy) 
        {
            Debug.Log("wrong stage of the game to trade in cards");
            return;
        }
        _placeableTroops += _uiManager.GetCardUIManager().TradeInCards(this);
    }    
}
