using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class HumanPlayer : Player
{
    private ITerritoryPlayerView _selectedTerritory = null;
    private ITerritoryPlayerView _previouslySelectedTerritory = null;

    public HumanPlayer(GameState state, PlayerData data, bool is2PlayerGame) : base(state, data, is2PlayerGame)
    {
        // Set the "trade in cards" button to call a function from this class
        // Needs to be done in script, because an object with this class attached does not exist in the scene before running it
        UIManager.Instance.GetCardUIManager().GetTradeInCardsButton().onClick.AddListener(TradeInCards);
        _hand = new List<TerritoryCard>();
        _data = data;
    }
    public void Update()
    {
        if(Input.GetMouseButtonDown(0) && _isMyTurn && !UIManager.Instance.PanelOverlayIsDisplayed)
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
                SelectTerritory(hit.collider.gameObject.GetComponent<Territory>().data);
                switch(_gameState.turnStage){
                    case TurnStage.InitDeploy:
                        new SetupDeploy(this, _selectedTerritory).execute();
                        break;
                    case TurnStage.InitReinforce:
                        new SetupDeploy(this, _selectedTerritory).execute();
                        break;
                    case TurnStage.Deploy:
                        if(_selectedTerritory.GetOwner().Equals(_data.playerName))
                        {
                            success = new Deploy(this, _selectedTerritory).execute();
                        }
                        break;
                    case TurnStage.Attack:
                        if(_selectedTerritory != null && _previouslySelectedTerritory != null)
                        {
                            if(_previouslySelectedTerritory.GetOwner().Equals(_data.playerName) && !_selectedTerritory.GetOwner().Equals(_data.playerName) && _selectedTerritory.IsANeighbor(_previouslySelectedTerritory) && _previouslySelectedTerritory.TroopCount > 1)
                            {
                                UIManager.Instance.DisplayAttackPanel(_previouslySelectedTerritory, _selectedTerritory);
                                SelectTerritory(null);
                            }
                        }
                        break;
                    case TurnStage.Reinforce:
                        if(_selectedTerritory != null && _previouslySelectedTerritory != null)
                        {
                            if(_selectedTerritory.GetOwner().Equals(_previouslySelectedTerritory.GetOwner()) && _selectedTerritory.IsANeighbor(_previouslySelectedTerritory))
                            {
                                UIManager.Instance.DisplayFortifyPanel(_previouslySelectedTerritory, _selectedTerritory);
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
        new UpdatePlaceableTroops(this).execute();
        _isMyTurn = true;
        UIManager.Instance.RedrawCardPanel(this);
    }
    public override void AddCardsToHand(List<TerritoryCard> cards)
    {
        if(cards == null) return;
        _hand = Enumerable.Concat(_hand, cards).ToList();
        
        for(int i = 0; i < cards.Count; i++)
        {
            UIManager.Instance.AddCardToPanel(cards[i]);
        }
    }

    private void SelectTerritory(ITerritoryPlayerView tr)
    {
        if(_selectedTerritory != null)
        {
            UIManager.Instance.HighlightTerritory(_selectedTerritory, false);
        }
        if(tr != null)
        {
            UIManager.Instance.HighlightTerritory(tr, true);
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
        UIManager.Instance.GetCardUIManager().TradeInCards(this);
    }    
}
