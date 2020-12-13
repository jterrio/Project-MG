﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    public static LevelManager lm;

    private Coroutine levelCoroutine;

    public Animator musicaAnim;
    private float waitTimeStart;

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

    public void LoadFarmDeath() {
        print("You died!");
        Destroy(GameManager.gm.player);
        StartCoroutine(LoadLevel("Sample"));
        UIManager.ui.enemyCount.gameObject.SetActive(false);
    }

    IEnumerator LoadLevel(string level) {
        waitTimeStart = Time.time;
        musicaAnim.SetTrigger("FadeOut");
        UIManager.ui.loadScreenPanel.gameObject.SetActive(true);
        AsyncOperation a = SceneManager.LoadSceneAsync(level);
        a.allowSceneActivation = false;
        while (a.progress < 0.9f) {
            UIManager.ui.loadBarFill.fillAmount = (a.progress / 1) / 2;
            yield return null;
        }
        UIManager.ui.loadBarFill.fillAmount = 0.5f;

        a.allowSceneActivation = true;
    }


    private void OnLevelWasLoaded(int level) {
        print("Level " + level.ToString() + " was loaded!");
        StartCoroutine(LevelLoading());
    }

    IEnumerator LevelLoading() {
        bool monstersLoaded = false;
        bool itemsLoaded = false;

        GameManager.gm.FindPlayer();
        RoomManager.rm.CreateFloorLayout();
        GameManager.gm.currentFloor++;
        GameManager.gm.LoadWeapon(GameManager.gm.wepID);
        UIManager.ui.loadBarFill.fillAmount += 0.2f; //total now 0.7

        while (true) {
            if (HazardManager.hm != null) {
                if (HazardManager.hm.hasFinishedLoading && !monstersLoaded) {
                    monstersLoaded = true;
                    UIManager.ui.loadBarFill.fillAmount += 0.1f;
                }
            }
            if (ItemManager.im != null) {
                if (ItemManager.im.hasFinishedLoading && !itemsLoaded) {
                    itemsLoaded = true;
                    UIManager.ui.loadBarFill.fillAmount += 0.1f;
                }
            }
            if(monstersLoaded && itemsLoaded) {
                break;
            }
            yield return null;
        }

        UIManager.ui.loadBarFill.fillAmount = 1.0f;
        while (Time.time < waitTimeStart + MusicManager.mm.waitTime) {
            yield return null;
        }



        MusicManager.mm.PlayBGM(MusicManager.mm.farmMusic);
        UIManager.ui.loadScreenPanel.gameObject.SetActive(false);


    }

}
