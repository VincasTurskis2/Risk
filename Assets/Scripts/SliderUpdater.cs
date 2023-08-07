using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderUpdater : MonoBehaviour
{
    private Slider _slider;
    [SerializeField]
    private TMP_InputField _inputField;
    void Awake()
    {
        _slider = gameObject.GetComponent<Slider>();
    }

    public void UpdateOccupyTroopSlider(string text)
    {
        int value = int.Parse(text);
        if(value > _slider.maxValue)
        {
            value = (int)_slider.maxValue;
            _inputField.text = value.ToString();
        }
        if(value < _slider.minValue)
        {
            value = (int)_slider.minValue;
            _inputField.text = value.ToString();
        }
        _slider.value = value;
    }
}
