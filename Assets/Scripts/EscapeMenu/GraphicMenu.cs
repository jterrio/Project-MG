using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicMenu : MonoBehaviour
{
    public Text fOVText;
    public Slider fOVSlider;
    public Camera MainCamera;

    private void Start() {
        setDefault();
        //Todo: set values from saved settings.
    }

    public void setDefault() {
        fOVText.text = MainCamera.fieldOfView.ToString();
        fOVSlider.value = MainCamera.fieldOfView;
    }

    public void UpdateFOV(float value) {
        fOVText.text = value.ToString();
        MainCamera.fieldOfView = value;
    }
}
