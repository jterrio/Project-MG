﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider verticalSlider;
    public InputField verticalInput;
    public Slider horizontalSlider;
    public InputField horizontalInput;

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateValueFromVerticalFloat(float value) {
        Debug.Log("float value changed: " + value);
        if (verticalSlider) { verticalSlider.value = value; }
        if (verticalInput) { verticalInput.text = value.ToString(); }
    }

    public void UpdateValueFromVerticalString(string value) {
        Debug.Log("string value changed: " + value);
        if (verticalSlider) { verticalSlider.value = float.Parse(value); }
        if (verticalInput) { verticalInput.text = value; }
    }

    public void UpdateValueFromHorizontalFloat(float value) {
        Debug.Log("float value changed: " + value);
        if (horizontalSlider) { horizontalSlider.value = value; }
        if (horizontalInput) { horizontalInput.text = value.ToString(); }
    }

    public void UpdateValueFromHorizontalString(string value) {
        Debug.Log("string value changed: " + value);
        if (horizontalSlider) { horizontalSlider.value = float.Parse(value); } 
        if (horizontalInput) { horizontalInput.text = value; }
    }
}
