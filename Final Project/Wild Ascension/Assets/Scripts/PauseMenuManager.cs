using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;     

public class PauseMenuManager : MonoBehaviour {
    [Header("UI Panels")]
    public GameObject pauseMenuPanel;   // drag your PauseMenuPanel here
    public GameObject controlsPanel;    // drag your ControlsPanel here

    private bool isPaused = false;

    void Start() {
        // load or create save after one frame so other scripts finish start()
        StartCoroutine(DeferredLoad());

        // Hide menus and unpause the game
        Time.timeScale = 1f;
        isPaused = false;
        if (pauseMenuPanel != null) {
            pauseMenuPanel.SetActive(false);
        }
        if (controlsPanel  != null) {
            controlsPanel.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    // Wait one frame so all other Start() methods complete first
    IEnumerator DeferredLoad() {
        yield return null;  
        bool loaded = SaveLoadManager.LoadGame();
        if (!loaded) {
            SaveLoadManager.SaveGame();
            // Debugging
            // Debug.Log("DeferredLoad: No save found → created new save.");
        }
    }

    void Update() {
        // toggle pause using the escape key
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!isPaused) {
                PauseGame();
            } 
            else {
                if (controlsPanel != null && controlsPanel.activeSelf) {
                    OpenPauseMenu();
                }
                else {
                    ResumeGame();
                }
            }
        }
    }

    private void PauseGame() {
        Time.timeScale = 0f;
        isPaused = true;
        if (pauseMenuPanel != null) {
            pauseMenuPanel.SetActive(true);
        }
        if (controlsPanel  != null) {
            controlsPanel.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    public void ResumeGame() {
        if (pauseMenuPanel != null) {
            pauseMenuPanel.SetActive(false);
        }
        if (controlsPanel  != null) { 
            controlsPanel.SetActive(false);
        }
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    public void OpenControlsMenu() {
        if (pauseMenuPanel != null) {
            pauseMenuPanel.SetActive(false);
        }
        if (controlsPanel  != null) {
            controlsPanel.SetActive(true);
        }
    }

    public void OpenPauseMenu() {
        if (controlsPanel  != null) {
            controlsPanel.SetActive(false);
        }
        if (pauseMenuPanel != null) {
            pauseMenuPanel.SetActive(true);
        }
    }

    // Hooked up tto the UI buttons
    public void OnSaveGamePressed()   => SaveLoadManager.SaveGame();
    public void OnDeleteSavePressed() {
        SaveLoadManager.DeleteSave();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void OnControlsPressed() => OpenControlsMenu();
    public void OnBackPressed() => OpenPauseMenu();
    public void OnResumePressed() => ResumeGame();
}
