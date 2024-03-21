using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// A class in the main menu that manages the player selection menu. Is not destroyed on loading the game scene, and information is taken from this class in order to choose the right players
public class PlayerSelectionManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerMenuPrefab;

    [SerializeField]
    private Button _addPlayerButton;
    [SerializeField]
    private TextMeshProUGUI _playerCountText;
    [SerializeField]
    private  GameObject _playerSelectionPanel;
    [SerializeField]
    private Canvas _canvas;
    public PlayerData[] playerData;

    // A list of player menus open in the scene. Make sure to drag in the menus that exist in editor before playing!
    [field: SerializeField]
    public List<PlayerSelectorMenu> PlayerMenus{get; private set;}
    public int simulationRuns = 20;
    public bool simulation = false;

    void Start()
    {
        simulationRuns = 3;
        DontDestroyOnLoad(this.gameObject);
        foreach (PlayerSelectorMenu psm in PlayerMenus)
        {
            psm.Setup(this);
        }
        CheckButtonEnable();
    }

    public void LaunchGame()
    {
        simulation = false;
        playerData = new PlayerData[PlayerMenus.Count];
        for(int i = 0; i < playerData.Length; i++)
        {
            playerData[i] = PlayerMenus[i].ReadData();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void LaunchSimulation()
    {
        simulation = true;
        playerData = new PlayerData[PlayerMenus.Count];
        for (int i = 0; i < playerData.Length; i++)
        {
            playerData[i] = PlayerMenus[i].ReadData();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void AddNewMenu()
    {
        Vector3 newPosition = PlayerMenus[PlayerMenus.Count - 1].transform.position;
        newPosition.y -= 60 * _canvas.scaleFactor;
        GameObject instantiatedMenu = Instantiate(_playerMenuPrefab, newPosition, Quaternion.identity, _playerSelectionPanel.transform);
        PlayerSelectorMenu newMenu = instantiatedMenu.GetComponent<PlayerSelectorMenu>();
        newMenu.Setup(this);
        PlayerMenus.Add(newMenu);
        _playerCountText.text = "Players (" + PlayerMenus.Count + "/6)";
        CheckButtonEnable();
        
    }

    public void DeleteMenu(PlayerSelectorMenu toDelete)
    {
        int index = PlayerMenus.IndexOf(toDelete);
        for(int i = index + 1; i < PlayerMenus.Count; i++)
        {
            PlayerMenus[i].transform.Translate(new Vector3(0, 60 * _canvas.scaleFactor, 0));
        }
        PlayerMenus.Remove(toDelete);
        Destroy(toDelete.gameObject);

        _playerCountText.text = "Players (" + PlayerMenus.Count + "/6)";
        CheckButtonEnable();
        
    }

    public void CheckButtonEnable()
    {
        if(PlayerMenus.Count >= 6)
        {
            _addPlayerButton.interactable = false;
        }
        else
        {
            _addPlayerButton.interactable = true;
        }

        if(PlayerMenus.Count <= 2)
        {
            foreach (PlayerSelectorMenu psm in PlayerMenus)
            {
                psm.removeButton.interactable = false;
            }
        }
        else
        {
            foreach (PlayerSelectorMenu psm in PlayerMenus)
            {
                psm.removeButton.interactable = true;
            }
        }
    }
}
