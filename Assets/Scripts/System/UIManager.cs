using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager ui;

    [Header("Loading Screen")]
    public Image loadBarFill;
    public RectTransform loadScreenPanel;

    [Header("Main Menu")]
    public RectTransform mainMenuPanel;

    private void Start() {
        if(ui == null) {
            ui = this;
        }else if(ui != this) {
            Destroy(this);
        }
    }

    public void DisableMainMenuUI() {
        mainMenuPanel.gameObject.SetActive(false);
    }

    public void EnableMainMenuUI() {
        mainMenuPanel.gameObject.SetActive(true);
    }





}
