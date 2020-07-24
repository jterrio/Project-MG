using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    public static LevelManager lm;

    private Coroutine levelCoroutine;

    [Header("Main Menu")]
    public Camera mainMenuCamera;
    public MainMenuCamera mmc;
    public SunEffect se;

    // Start is called before the first frame update
    void Start() {
        if (lm == null) {
            lm = this;
        } else if (lm != this) {
            Destroy(this);
        }
    }

    public void MainMenuPlayTransition() {
        CleanCoroutine();
        levelCoroutine = StartCoroutine("MainMenuPlayTransitionAnimation");
    }


    IEnumerator MainMenuPlayTransitionAnimation() {
        UIManager.ui.DisableMainMenuUI();
        se.SetDay();
        while (true) {
            mainMenuCamera.gameObject.transform.position = Vector3.MoveTowards(mainMenuCamera.gameObject.transform.position, GameManager.gm.player.transform.position, mmc.moveSpeed * Time.deltaTime);
            mainMenuCamera.gameObject.transform.rotation = Quaternion.Lerp(mainMenuCamera.gameObject.transform.rotation, GameManager.gm.playerCamera.transform.rotation, mmc.rotationSpeed * Time.deltaTime);
            if (Vector3.Distance(mainMenuCamera.gameObject.transform.position, GameManager.gm.player.transform.position) <= 1f) {
                break;
            }
            yield return null;
        }
        mainMenuCamera.gameObject.SetActive(false);
        GameManager.gm.player.SetActive(true);
        CleanCoroutine();
    }

    void CleanCoroutine() {
        if(levelCoroutine != null) {
            StopCoroutine(levelCoroutine);
        }
        levelCoroutine = null;
    }


    public void LoadFarmLevel() {
        print("Changing level to Farm...");
        StartCoroutine(LoadLevel("Sample"));
    }

    IEnumerator LoadLevel(string level) {
        UIManager.ui.loadScreenPanel.gameObject.SetActive(true);
        AsyncOperation a = SceneManager.LoadSceneAsync(level);
        //a.allowSceneActivation = false;
        while (a.progress < 0.9) {
            UIManager.ui.loadBarFill.fillAmount = (a.progress / 1);
            yield return null;
        }
        //a.allowSceneActivation = true;
        RoomManager.rm.CreateFloorLayout();
        GameManager.gm.currentFloor++;
        UIManager.ui.loadScreenPanel.gameObject.SetActive(false);
    }


    private void OnLevelWasLoaded(int level) {
        GameManager.gm.FindPlayer();
    }

}
