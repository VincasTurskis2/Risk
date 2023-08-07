using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InputFieldUpdater : MonoBehaviour
{
    private TMP_InputField _field;
    [SerializeField]
    private Slider _slider;

    void Awake()
    {
        _field = gameObject.GetComponent<TMP_InputField>();
    }
    public void UpdateField(float _sliderValue)
    {
        int value = (int) _sliderValue;
        _field.text = value.ToString();
    }
}
