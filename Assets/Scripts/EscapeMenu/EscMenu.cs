using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject escapeMenuUI;
    public GameObject pauseMenu;
    public GameObject optionMenu;
    public GameObject graphicMenu;
    public GameObject controlMenu;
    public Scene MainMenu;
    public menuType whereAmI;

    public enum menuType {
        PAUSE,
        MAIN_OPTIONS,
        GRAPHIC_OPTIONS,
        CONTROL_OPTIONS
    }
    // Update is called once per frame
    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab)) {
            if (GameIsPaused) {
                Resume();
            } else {
                Pause();
            }
        }
    }

    public void Resume() {
        escapeMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        GameIsPaused = false;
    }

    public void UpdateMenu() {
        switch (whereAmI) {
            case menuType.PAUSE:

                break;
            case menuType.MAIN_OPTIONS:
                break;
            case menuType.GRAPHIC_OPTIONS:
                break;
            case menuType.CONTROL_OPTIONS:
                break;
        }
    }

    void Pause() {
        escapeMenuUI.SetActive(true);
        whereAmI = menuType.PAUSE;
        UpdateMenu();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMainMenu() {
        Debug.Log("Loading Main Menu....");
        SceneManager.LoadScene(MainMenu.ToString());
    }

    public void OptionsMenu() {
        pauseMenu.SetActive(false);
        optionMenu.SetActive(true);
    }

    public void QuitGame() {
        Debug.Log("Quiting Game now, Good Bye");

        //ToDo: Bring up prompt to say: Are you sure? Any unsaved progress will be lost.
        if (true) {
            Application.Quit();
        }
    }
}
