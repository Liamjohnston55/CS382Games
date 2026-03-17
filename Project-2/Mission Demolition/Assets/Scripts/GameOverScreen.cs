using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    // This method is called by the Restart or Back To Start button.
    public void RestartGame()
    {
        // Replace "StartScreen" with the name of your start screen scene
        SceneManager.LoadScene("StartScreen");
    }
}
