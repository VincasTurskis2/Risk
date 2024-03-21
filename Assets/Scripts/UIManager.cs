using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System;

// A class 
public class UIManager : MonoBehaviour
{

    public static UIManager Instance {get; private set;}
    void Awake()
    {
        Instance = this;
    }
    private UIManager()
    {

    }

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

    [SerializeField]
    private GameObject _winLosePanel;
    [SerializeField]
    private TextMeshProUGUI _turnCount;
    [SerializeField]
    private TextMeshProUGUI _gameTimeText;
    [SerializeField]
    private TextMeshProUGUI _winnerText;

    public bool PanelOverlayIsDisplayed {get; private set;} = false;

    private int _cumulativeAttackerLoss, _cumulativeDefenderLoss;

    void Update()
    {
        if(!GameMaster.Instance.isMCTSSimulation)
        {
            UpdateCurrentStageText();
            UpdateCurrentPlayerText();
            UpdateHelperPanelText();
        }
    }
    public void Setup()
    {
        _cardUIManager.gameObject.SetActive(true);
        _winLosePanel.SetActive(false);
        _cumulativeAttackerLoss = 0;
        _cumulativeDefenderLoss = 0;
        HideAttackPanel();
        HideFortifyPanel();
        _cardUIManager.Setup();
        
    }
    public void UpdateCurrentStageText()
    {
        switch(GameMaster.Instance.state.turnStage){
            case TurnStage.InitDeploy:
                _currentStageText.SetText("Current Stage: Setup");
                break;
            case TurnStage.InitReinforce:
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
        _currentPlayerText.SetText(GameMaster.Instance.CurrentPlayer().GetData().playerName + "'s turn");
    }
    public void UpdateHelperPanelText()
    {
        _helperPanel.SetActive(GameMaster.Instance.state.turnStage == TurnStage.Deploy || GameMaster.Instance.state.turnStage == TurnStage.InitDeploy || GameMaster.Instance.state.turnStage == TurnStage.InitReinforce);
        _helperPanelText.SetText("Troops left to deploy: " + GameMaster.Instance.CurrentPlayer().GetPlaceableTroopNumber());
    }
    public void DisplayAttackPanel(ITerritoryPlayerView from, ITerritoryPlayerView to)
    {
        if(GameMaster.Instance.isMCTSSimulation == false && GameMaster.Instance.isAIOnlyGame == false)
        {
            _attackPanel.SetActive(true);
            _cumulativeAttackerLoss = 0;
            _cumulativeDefenderLoss = 0;
            _resultText.gameObject.SetActive(false);
            _endStageButton.gameObject.SetActive(false);
            _occupyTroopSlider.gameObject.SetActive(false);
            _cardUIManager.gameObject.SetActive(false);
            _retreatButton.interactable = true;
            _attackButton.interactable = true;
            _attackPanelTitle.SetText("Battle for " + to.TerritoryName);
            _attackButtonText.SetText("Attack!");

            _attackerName.SetText("Attacker: " + from.GetOwner());
            _attackerOrigin.SetText("(" + from.TerritoryName + ")");
            _attackerRemainingTroops.SetText("Troops remaining: " + (from.TroopCount - 1));

            _defenderName.SetText("Defender: " + to.GetOwner());
            _defenderRemainingTroops.SetText("Troops Remaining: " + to.TroopCount);

            _attackerDice.SetText("Dice rolled: {}");
            _attackerTroopLoss.SetText("Troops lost: 0");
            _attackerTotalLoss.SetText("Total troops lost: " + _cumulativeAttackerLoss);

            _defenderDice.SetText("Dice rolled: {}");
            _defenderTroopLoss.SetText("Troops lost: 0");
            _defenderTotalLoss.SetText("Total troops lost: " + _cumulativeDefenderLoss);

            _attackButton.onClick.RemoveAllListeners();
            _attackButton.onClick.AddListener(delegate {Attack(from, to);});
            PanelOverlayIsDisplayed = true;
        }
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
        if (GameMaster.Instance.isMCTSSimulation == false && GameMaster.Instance.isAIOnlyGame == false)
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
    }
    public void UpdateAttackPanelResults(ITerritoryPlayerView from, ITerritoryPlayerView to)
    {
        if (GameMaster.Instance.isMCTSSimulation == false && GameMaster.Instance.isAIOnlyGame == false)
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
                _attackButton.onClick.AddListener(delegate {Occupy(from, to, (int)_occupyTroopSlider.value);});
                _attackButton.onClick.AddListener(delegate {HideAttackPanel();});
            }
        }
    }
    public void HideFortifyPanel()
    {
        _endStageButton.interactable = true;
        _fortifySliderPanel.SetActive(false);
        PanelOverlayIsDisplayed = false;
    }
    public void DisplayFortifyPanel(ITerritoryPlayerView from, ITerritoryPlayerView to)
    {
        if (GameMaster.Instance.isMCTSSimulation == false && GameMaster.Instance.isAIOnlyGame == false)
        {
            _endStageButton.interactable = false;
            _fortifySliderPanel.SetActive(true);
            _fortifySlider.maxValue = from.TroopCount - 1;
            _fortifySlider.value = _fortifySlider.maxValue;
            _fortifySliderMaxText.text = _fortifySlider.maxValue.ToString();
            _fortifySliderInputField.text = _fortifySlider.value.ToString();
            _fortifySliderConfirmButton.onClick.RemoveAllListeners();
            _fortifySliderConfirmButton.onClick.AddListener(delegate {Fortify(from, to, (int)_fortifySlider.value);});
            _fortifySliderConfirmButton.onClick.AddListener(delegate {HideFortifyPanel();});
            PanelOverlayIsDisplayed = true;
        }
    }

    public void AddCardToPanel(TerritoryCard card)
    {
        if (GameMaster.Instance.isMCTSSimulation == false && GameMaster.Instance.isAIOnlyGame == false)
        {
            _cardUIManager.AddCard(card);
        }
    }
    public void RedrawCardPanel(Player player)
    {
        if (GameMaster.Instance.isMCTSSimulation == false && GameMaster.Instance.isAIOnlyGame == false)
        {
            _cardUIManager.RedrawCardHand(player);
        }
    }
    public CardUIManager GetCardUIManager()
    {
        return _cardUIManager;
    }

    public void DisplayVictoryPanel(string winnerName, int turnCount, float gameTimeSeconds)
    {
        if (GameMaster.Instance.isMCTSSimulation == false && GameMaster.Instance.isAIOnlyGame == false)
        {
            HideAttackPanel();
            HideFortifyPanel();
            _currentPlayerText.transform.parent.gameObject.SetActive(false);
            _currentStageText.transform.parent.gameObject.SetActive(false);
            _endStageButton.gameObject.SetActive(false);
            _cardUIManager.gameObject.SetActive(false);
            PanelOverlayIsDisplayed = true;

            _winnerText.text = winnerName + " wins!";

            _turnCount.text = "" + turnCount;

            string timeString = String.Format("{0:00}:{1:00}:{2:00}", (int) gameTimeSeconds / 3600, (int) gameTimeSeconds/60, (int) gameTimeSeconds % 60);

            _gameTimeText.text = timeString;

            _winLosePanel.SetActive(true);
        }
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public bool Attack(ITerritoryPlayerView from, ITerritoryPlayerView to)
    {
        return new Attack(GameMaster.Instance.state.getPlayerFromName(from.GetOwner()), from, to).execute();
    }
    public bool Fortify(ITerritoryPlayerView from, ITerritoryPlayerView to, int numberOfTroops)
    {
        return new Fortify(GameMaster.Instance.state.getPlayerFromName(from.GetOwner()), from, to, numberOfTroops).execute();
    }
    public bool Occupy(ITerritoryPlayerView from, ITerritoryPlayerView to, int numberOfTroops)
    {
        return new Occupy(GameMaster.Instance.state.getPlayerFromName(from.GetOwner()), from, to, numberOfTroops).execute();
    }

    
    public void HighlightTerritory(ITerritoryPlayerView territory, bool toHighlight)
    {
        if(territory == null) return;
        TerritoryData Territory = (TerritoryData) territory;
        if(territory.GetOwner() == null)
        {
            if (toHighlight)
            {
                Territory.territoryColor = Helpers.GetHighlighedColorVersion(ColorPreset.White);
            }
            else
            {
                Territory.territoryColor = ColorPreset.White;
            }
            return;
        }
        if(toHighlight)
        {
            Territory.territoryColor = Helpers.GetHighlighedColorVersion(GameMaster.Instance.state.getPlayerFromName(Territory.Owner).GetData().playerColor);
        }
        else
        {
            Territory.territoryColor = GameMaster.Instance.state.getPlayerFromName(Territory.Owner).GetData().playerColor;
        }
    }
}
