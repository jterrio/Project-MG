using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public static UIManager ui;

    public Image loadBarFill;
    public RectTransform loadScreenPanel;

    private void Start() {
        if(ui == null) {
            ui = this;
        }else if(ui != this) {
            Destroy(this);
        }
    }

    public void LoadFarmLevel() {
        StartCoroutine(LoadLevel("Sample"));
    }

    IEnumerator LoadLevel(string level) {
        loadScreenPanel.gameObject.SetActive(true);
        AsyncOperation a = SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);
        //a.allowSceneActivation = false;
        while(a.progress < 0.9) {
            loadBarFill.fillAmount = (a.progress / 1);
            yield return null;
        }
        //a.allowSceneActivation = true;

    }





}
