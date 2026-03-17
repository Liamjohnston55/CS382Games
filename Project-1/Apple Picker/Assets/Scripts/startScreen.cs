using ApplePicker;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startScreen : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void RestartGame() {
        // Delete the "game manager" to reset the level fully
        GameManager gm = FindObjectOfType<GameManager>(); // Find existing GameManager
        if (gm != null) {
            Destroy(gm.gameObject); // Destroy it 
        }
        SceneManager.LoadScene("StartMenu"); 
    }
}