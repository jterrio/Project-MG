using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicMenu : MonoBehaviour
{
    public Text fOVText;
    public Slider fOVSlider;

    private void Start() {
        textUpdate(fOVSlider.value);
    }

    public void textUpdate(float value) {
        fOVText.text = value.ToString();
    }
}
