using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance; // Singleton instance
    public static int lives = 5;        // Lives remaining (you can adjust this per level)
    public int startingLives = 5;       // this is to reset the lives at the beginning of each level 

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
    }

    void OnDestroy() {
        // to make sure there is no memory leaks 
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // this function is called when the player reaches the goal zone:
    public void CompleteLevel() {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings) {
            SceneManager.LoadScene(nextIndex);
        }
        else {
            // if all levels are complete go to the winning screen
            SceneManager.LoadScene("VictoryScreen");
        }
    }

    // this function if called when the player has no lives left
    public void GameOver() {
        SceneManager.LoadScene("EndScreen");
    }
}
