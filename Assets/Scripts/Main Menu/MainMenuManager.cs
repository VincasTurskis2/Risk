using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]

    private GameObject _playerSelectionPanel, _playButton, _exitButton, _titleText;
    
    void Start()
    {
        _playerSelectionPanel.SetActive(false);
        _playButton.SetActive(true);
        _exitButton.SetActive(true);
        _titleText.SetActive(true);
    }
    public void QuitGame()
    {
        Debug.Log("Quit game!");
        Application.Quit();
    }
    public void EnablePlayerSelectionPanel()
    {
        _playerSelectionPanel.SetActive(true);
        _playButton.SetActive(false);
        _exitButton.SetActive(false);
        _titleText.SetActive(false);
    }
    public void DisablePlayerSelectionPanel()
    {
        _playerSelectionPanel.SetActive(false);
        _playButton.SetActive(true);
        _exitButton.SetActive(true);
        _titleText.SetActive(true);
    }
    
}
