using UnityEngine;

/**
 * Learning resources:
 * Brackey Pause Menu tutorial: https://youtu.be/JivuXdrIHK0
 * Unlock mouse: https://forum.unity.com/threads/locking-and-unlocking-mouse-cursor-unity.367172/
 * 
 */

public class PauseMenu : MonoBehaviour {

    public GameObject pauseMenuUI;
    private static bool gameIsPaused;

    void Update() {

        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        
        if (!gameIsPaused) {
            Pause();
        }
        else {
            Resume();
        }
    }

    public void Resume() {
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resume the game.
        gameIsPaused = false;
    }

    private void Pause() {

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Freeze the game.
        gameIsPaused = true;
    }

    public void QuitGame() {
        
        Application.Quit();
    }
}
