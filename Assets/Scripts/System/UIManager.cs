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

    [Header("HUD")]
    public RectTransform HUD;
    public RectTransform miniMap;
    public RectTransform crosshair;
    public RectTransform ammoCount;
    public RectTransform HealthUIFill;
    public RectTransform enemyCount;
    public RectTransform moneyCount;

    private void Start() {
        if(ui == null) {
            ui = this;
        }else if(ui != this) {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }

    public void DisableMainMenuUI() {
        mainMenuPanel.gameObject.SetActive(false);
    }

    public void EnableMainMenuUI() {
        mainMenuPanel.gameObject.SetActive(true);
    }

    public void TurnOnHUD() {
        HUD.gameObject.SetActive(true);
    }

    public void TurnOffHUD() {
        HUD.gameObject.SetActive(false);
    }





}
