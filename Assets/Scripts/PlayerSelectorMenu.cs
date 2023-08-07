using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerSelectorMenu : MonoBehaviour
{

    [SerializeField]
    private TMP_Dropdown _playerTypeSelection;
    [SerializeField]
    private TMP_Dropdown _colorSelection;
    [SerializeField]
    private TMP_InputField _nameInputField;
    private PlayerSelectionManager _manager;
    public Button removeButton;
    public void Setup(PlayerSelectionManager gsm)
    {
        _manager = gsm;
        _playerTypeSelection.ClearOptions();
        List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
        foreach(PlayerType t in Enum.GetValues(typeof(PlayerType)))
        {
            switch(t)
            {
                case PlayerType.Human:
                    list.Add(new TMP_Dropdown.OptionData("Human"));
                    break;
                case PlayerType.PassiveAI:
                    list.Add(new TMP_Dropdown.OptionData("Passive AI"));
                    break;
            }
        }
        _playerTypeSelection.AddOptions(list);
        _playerTypeSelection.value = gsm.playerMenus.IndexOf(this);
    }
    public PlayerData ReadData()
    {
        String name;
        PlayerType type;
        Color color;

        name = _nameInputField.text;
        type = (PlayerType) _playerTypeSelection.value;
        color = ColorPreset.GetColorPreset(_colorSelection.value);

        return new PlayerData(name, type, color);
    }

    public void DeleteMenu()
    {
        _manager.DeleteMenu(this);
    }

}
