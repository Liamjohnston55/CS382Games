using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; 

public class InfoHub : MonoBehaviour {
    [Header("Level Info")]
    public TextMeshProUGUI livesText; // Drag the current amount of lives here so the player can see it
    public TextMeshProUGUI currentLevelText; // Drag the current level here so the player can see it

    void Update() {
        // Display the current amount of lives:
        livesText.text = "Lives: " + GameManager.lives;

        // Display the current level: 
        int levelIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        currentLevelText.text = "Level: " + levelIndex;
    }
}
