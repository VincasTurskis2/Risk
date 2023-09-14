using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

// A class 
public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _currentStageText;
    [SerializeField]
    private TextMeshProUGUI _currentPlayerText;
    [SerializeField]
    private GameObject _helperPanel;
    [SerializeField]
    private TextMeshProUGUI _helperPanelText;
    [SerializeField]
    private Button _endStageButton;


    [SerializeField]
    private Button _attackButton;
    [SerializeField]
    private TextMeshProUGUI _attackButtonText;
    [SerializeField]
    private Button _retreatButton;


    [SerializeField]
    private GameObject _attackPanel;
    [SerializeField]
    private TextMeshProUGUI _attackPanelTitle;
    [SerializeField]
    private TextMeshProUGUI _resultText;

    [SerializeField]
    private TextMeshProUGUI _attackerName;
    [SerializeField]
    private TextMeshProUGUI _attackerOrigin;
    [SerializeField]
    private TextMeshProUGUI _attackerRemainingTroops;
    [SerializeField]
    private TextMeshProUGUI _attackerDice;
    [SerializeField]
    private TextMeshProUGUI _attackerTroopLoss;
    [SerializeField]
    private TextMeshProUGUI _attackerTotalLoss;

    [SerializeField]
    private TextMeshProUGUI _defenderName;
    [SerializeField]
    private TextMeshProUGUI _defenderRemainingTroops;
    [SerializeField]
    private TextMeshProUGUI _defenderDice;
    [SerializeField]
    private TextMeshProUGUI _defenderTroopLoss;
    [SerializeField]
    private TextMeshProUGUI _defenderTotalLoss;


    [SerializeField]
    private TextMeshProUGUI _occupyTroopSliderMinText;
    [SerializeField]
    private TextMeshProUGUI _occupyTroopSliderMaxText;
    [SerializeField]
    private Slider _occupyTroopSlider;
    [SerializeField]
    private TMP_InputField _occupyTroopInputField;


    [SerializeField]
    private GameObject _fortifySliderPanel;
    [SerializeField]
    private Slider _fortifySlider;
    [SerializeField]
    private TMP_InputField _fortifySliderInputField;
    [SerializeField]
    private TextMeshProUGUI _fortifySliderMaxText;
    [SerializeField]
    private Button _fortifySliderConfirmButton;


    [SerializeField]
    private CardUIManager _cardUIManager;



    private GameState _gameState;
    private PlayerActions _playerActions;

    public bool PanelOverlayIsDisplayed {get; private set;} = false;

    private int _cumulativeAttackerLoss, _cumulativeDefenderLoss;

    void Update()
    {
        UpdateCurrentStageText();
        UpdateCurrentPlayerText();
        UpdateHelperPanelText();
    }
    public void Setup()
    {
        _gameState = gameObject.GetComponent<GameState>();
        _playerActions = gameObject.GetComponent<PlayerActions>();
        _cumulativeAttackerLoss = 0;
        _cumulativeDefenderLoss = 0;
        HideAttackPanel();
        HideFortifyPanel();
        
    }
    public void UpdateCurrentStageText()
    {
        switch(_gameState.turnStage){
            case TurnStage.Setup:
                _currentStageText.SetText("Current Stage: Setup");
                break;
            case TurnStage.Deploy:
                _currentStageText.SetText("Current Stage: Deploy");
                break;
            case TurnStage.Attack:
                _currentStageText.SetText("Current Stage: Attack");
                break;
            case TurnStage.Reinforce:
                _currentStageText.SetText("Current Stage: Reinforce");
                break;
        }
    }
    public void UpdateCurrentPlayerText()
    {
        _currentPlayerText.SetText(_gameState.CurrentPlayer().GetData().playerName + "'s turn");
    }
    public void UpdateHelperPanelText()
    {
        _helperPanel.SetActive(_gameState.turnStage == TurnStage.Deploy || _gameState.turnStage == TurnStage.Setup);
        _helperPanelText.SetText("Troops left to deploy: " + _gameState.CurrentPlayer().GetPlaceableTroopNumber());
    }
    public void DisplayAttackPanel(Territory from, Territory to)
    {
        _attackPanel.SetActive(true);
        _cumulativeAttackerLoss = 0;
        _cumulativeDefenderLoss = 0;
        _resultText.gameObject.SetActive(false);
        _endStageButton.gameObject.SetActive(false);
        _occupyTroopSlider.gameObject.SetActive(false);
        _cardUIManager.gameObject.SetActive(false);
        _attackButton.interactable = true;
        _attackPanelTitle.SetText("Battle for " + to.name);
        _attackButtonText.SetText("Attack!");

        _attackerName.SetText("Attacker: " + from.Owner.GetData().playerName);
        _attackerOrigin.SetText("(" + from.name + ")");
        _attackerRemainingTroops.SetText("Troops remaining: " + (from.TroopCount - 1));

        _defenderName.SetText("Defender: " + to.Owner.GetData().playerName);
        _defenderRemainingTroops.SetText("Troops Remaining: " + to.TroopCount);

        _attackerDice.SetText("Dice rolled: {}");
        _attackerTroopLoss.SetText("Troops lost: 0");
        _attackerTotalLoss.SetText("Total troops lost: " + _cumulativeAttackerLoss);

        _defenderDice.SetText("Dice rolled: {}");
        _defenderTroopLoss.SetText("Troops lost: 0");
        _defenderTotalLoss.SetText("Total troops lost: " + _cumulativeDefenderLoss);

        _attackButton.onClick.RemoveAllListeners();
        _attackButton.onClick.AddListener(delegate {_playerActions.Attack(from, to);});
        PanelOverlayIsDisplayed = true;
    }
    public void HideAttackPanel()
    {
        _currentPlayerText.transform.parent.gameObject.SetActive(true);
        _currentStageText.transform.parent.gameObject.SetActive(true);
        _endStageButton.gameObject.SetActive(true);
        _cardUIManager.gameObject.SetActive(true);
        PanelOverlayIsDisplayed = false;
        _attackPanel.SetActive(false);
    }
    public void UpdateAttackPanelNumbers(int[][] diceRolls, int[]diceRollResults)
    {
        _attackerDice.SetText("Dice rolled: " + Helpers.IntArrayToString(diceRolls[0]));
        _attackerTroopLoss.SetText("Troops lost: " + diceRollResults[0]);
        _cumulativeAttackerLoss += diceRollResults[0];
        _attackerTotalLoss.SetText("Total troops lost: " + _cumulativeAttackerLoss);

        _defenderDice.SetText("Dice rolled: " + Helpers.IntArrayToString(diceRolls[1]));
        _defenderTroopLoss.SetText("Troops lost: " + diceRollResults[1]);
        _cumulativeDefenderLoss += diceRollResults[1];
        _defenderTotalLoss.SetText("Total troops lost: " + _cumulativeDefenderLoss);
        _occupyTroopSlider.minValue = diceRollResults[2];
    }
    public void UpdateAttackPanelResults(Territory from, Territory to)
    {
        _attackerRemainingTroops.SetText("Troops remaining: " + (from.TroopCount - 1));
        _defenderRemainingTroops.SetText("Troops Remaining: " + to.TroopCount);
        if(from.TroopCount == 1)
        {
            _resultText.SetText("Defeat!");
            _resultText.gameObject.SetActive(true);
            _attackButton.interactable = false;
            _retreatButton.interactable = true;
        }
        if(to.TroopCount == 0)
        {
            _resultText.SetText("Victory!");
            _resultText.gameObject.SetActive(true);
            _retreatButton.interactable = false;
            _attackButtonText.SetText("Confirm");

            _occupyTroopSlider.maxValue = from.TroopCount - 1;
            _occupyTroopSliderMinText.SetText(_occupyTroopSlider.minValue.ToString());
            _occupyTroopSliderMaxText.SetText(_occupyTroopSlider.maxValue.ToString());
            
            _occupyTroopSlider.value = _occupyTroopSlider.maxValue;
            _occupyTroopInputField.text = _occupyTroopSlider.value.ToString();

            _occupyTroopSlider.gameObject.SetActive(true);

            _attackButton.onClick.RemoveAllListeners();
            _attackButton.onClick.AddListener(delegate {_playerActions.Occupy(from, to, (int)_occupyTroopSlider.value);});
            _attackButton.onClick.AddListener(delegate {HideAttackPanel();});
        }
    }
    public void HideFortifyPanel()
    {
        _fortifySliderPanel.SetActive(false);
        PanelOverlayIsDisplayed = false;
    }
    public void DisplayFortifyPanel(Territory from, Territory to)
    {
        _fortifySliderPanel.SetActive(true);
        _fortifySlider.maxValue = from.TroopCount - 1;
        _fortifySlider.value = _fortifySlider.maxValue;
        _fortifySliderMaxText.text = _fortifySlider.maxValue.ToString();
        _fortifySliderInputField.text = _fortifySlider.value.ToString();
        _fortifySliderConfirmButton.onClick.RemoveAllListeners();
        _fortifySliderConfirmButton.onClick.AddListener(delegate {_playerActions.Fortify(from, to, (int)_fortifySlider.value);});
        _fortifySliderConfirmButton.onClick.AddListener(delegate {HideFortifyPanel();});
        PanelOverlayIsDisplayed = true;
    }

    public void AddCardToPanel(TerritoryCard card)
    {
        _cardUIManager.AddCard(card);
    }
}
