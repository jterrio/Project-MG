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
        SETTINGS,
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
                LoadPause();
                UnLoadSettings();
                UnLoadGraphics();
                UnLoadControls();
                break;
            case menuType.SETTINGS:
                LoadSettings();
                UnLoadGraphics();
                UnLoadControls();
                UnLoadPause();
                break;
            case menuType.GRAPHIC_OPTIONS:
                LoadGraphics();
                UnLoadControls();
                UnLoadPause();
                UnLoadSettings();
                break;
            case menuType.CONTROL_OPTIONS:
                LoadControls();
                UnLoadPause();
                UnLoadSettings();
                UnLoadGraphics();
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

    // Methods for buttons to use when pressed.
    public void NavigatePauseMenu() {
        whereAmI = menuType.PAUSE;
        UpdateMenu();
    }
    public void NavigateSettingsMenu() {
        whereAmI = menuType.SETTINGS;
        UpdateMenu();
    }
    public void NavigateGraphicMenu() {
        whereAmI = menuType.GRAPHIC_OPTIONS;
        UpdateMenu();
    }
    public void NavigateControlMenu() {
        whereAmI = menuType.CONTROL_OPTIONS;
        UpdateMenu();
    }

    public void QuitGame() {
        Debug.Log("Quiting Game now, Good Bye");

        //ToDo: Bring up prompt to say: Are you sure? Any unsaved progress will be lost.
        if (true) {
            Application.Quit();
        }
    }

    //So loading an unloading the menus is easy.
    public void LoadSettings() {
        optionMenu.SetActive(true);
    }

    public void UnLoadSettings() {
        optionMenu.SetActive(false);
    }

    public void LoadPause() {
        pauseMenu.SetActive(true);
    }

    public void UnLoadPause() {
        pauseMenu.SetActive(false);
    }

    public void LoadGraphics() {
        graphicMenu.SetActive(true);
    }

    public void UnLoadGraphics() {
        graphicMenu.SetActive(false);
    }

    public void LoadControls() {
        controlMenu.SetActive(true);
    }

    public void UnLoadControls() {
        controlMenu.SetActive(false);
    }
}
