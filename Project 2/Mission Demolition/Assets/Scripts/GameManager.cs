using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance; // Singleton instance
    [Header("The actual amount of lives is: lives + 1")]
    public static int lives = 4;        // Lives remaining (you can adjust this per level)
    public int startingLives = 4;       // this is to reset the lives at the beginning of each level 
    public static bool levelCompleted = false;  // flag to indicate if level is completed


    void Awake(){
            if (Instance == null) {
                // Ensure only one gameManger exists
                // This was a pain in the last one so just add this here and do not touch
                Instance = this;
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else {
                Destroy(gameObject);
            }
        }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        // Reset lives for the new level when the scene is loaded
        lives = startingLives;
        levelCompleted = false;
    }

    void OnDestroy() {
        // to make sure there is no memory leaks 
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // this function is called when the player reaches the goal zone:
    public void CompleteLevel() {
        levelCompleted = true; // Mark the level as completed
        Scene currentScene = SceneManager.GetActiveScene();

        // If we are at the last level, go to the winning screen
        if (currentScene.name == "Level4") {
            SceneManager.LoadScene("VictoryScreen");
        }
        else {
            int nextIndex = currentScene.buildIndex + 1;
            if (nextIndex < SceneManager.sceneCountInBuildSettings) {
                SceneManager.LoadScene(nextIndex);
            }
            else {
                // failsafe 
                SceneManager.LoadScene("VictoryScreen");
            }
        }
    }

    // this function if called when the player has no lives left
    public void GameOver() {
        SceneManager.LoadScene("EndScreen");
    }
}
