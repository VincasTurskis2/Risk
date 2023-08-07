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
    private GameObject _playerSelectionPanel;
    public PlayerData[] playerData;

    // A list of player menus open in the scene. Make sure to drag in the menus that exist in editor before playing!
    [field: SerializeField]
    public List<PlayerSelectorMenu> playerMenus{get; private set;}

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        foreach (PlayerSelectorMenu psm in playerMenus)
        {
            psm.Setup(this);
        }
        CheckButtonEnable();
    }

    public void LaunchGame()
    {
        playerData = new PlayerData[playerMenus.Count];
        for(int i = 0; i < playerData.Length; i++)
        {
            playerData[i] = playerMenus[i].ReadData();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void AddNewMenu()
    {
        Vector3 newPosition = playerMenus[playerMenus.Count - 1].transform.position;
        newPosition.y = newPosition.y - 60;
        GameObject instantiatedMenu = Instantiate(_playerMenuPrefab, newPosition, Quaternion.identity, _playerSelectionPanel.transform);
        PlayerSelectorMenu newMenu = instantiatedMenu.GetComponent<PlayerSelectorMenu>();
        newMenu.Setup(this);
        playerMenus.Add(newMenu);
        _addPlayerButton.transform.Translate(new Vector3(0, -60, 0));
        _playerCountText.text = "Players (" + playerMenus.Count + "/6)";
        CheckButtonEnable();
        
    }

    public void DeleteMenu(PlayerSelectorMenu toDelete)
    {
        int index = playerMenus.IndexOf(toDelete);
        for(int i = index + 1; i < playerMenus.Count; i++)
        {
            playerMenus[i].transform.Translate(new Vector3(0, 60, 0));
        }
        _addPlayerButton.transform.Translate(new Vector3(0, 60, 0));
        playerMenus.Remove(toDelete);
        Destroy(toDelete.gameObject);

        _playerCountText.text = "Players (" + playerMenus.Count + "/6)";
        CheckButtonEnable();
        
    }

    public void CheckButtonEnable()
    {
        if(playerMenus.Count >= 6)
        {
            _addPlayerButton.interactable = false;
        }
        else
        {
            _addPlayerButton.interactable = true;
        }

        if(playerMenus.Count <= 2)
        {
            foreach (PlayerSelectorMenu psm in playerMenus)
            {
                psm.removeButton.interactable = false;
            }
        }
        else
        {
            foreach (PlayerSelectorMenu psm in playerMenus)
            {
                psm.removeButton.interactable = true;
            }
        }
    }
}
