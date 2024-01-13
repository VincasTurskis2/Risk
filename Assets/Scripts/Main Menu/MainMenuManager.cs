using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]

    private GameObject _playerSelectionPanel, _playButton, _simulationButton, _exitButton, _titleText, _simulationSettingsPanel;
    
    void Start()
    {
        ShowMainMenu();
    }
    public void QuitGame()
    {
        Debug.Log("Quit game!");
        Application.Quit();
    }

    public void HideAllPanels()
    {
        _playButton.SetActive(false);
        _exitButton.SetActive(false);
        _titleText.SetActive(false);
        _playerSelectionPanel.SetActive(false);
        _simulationSettingsPanel.SetActive(false);
        _simulationButton.SetActive(false);
    }
    public void ShowMainMenu()
    {
        HideAllPanels();
        _playButton.SetActive(true);
        _exitButton.SetActive(true);
        _titleText.SetActive(true);
        _simulationButton.SetActive(true);
    }
    public void ShowPlayerSelectionPanel()
    {
        HideAllPanels();
        _playerSelectionPanel.SetActive(true);
    }
    public void ShowSimulationSettingsPanel()
    {
        HideAllPanels();
        _simulationSettingsPanel.SetActive(true);
    }
}
