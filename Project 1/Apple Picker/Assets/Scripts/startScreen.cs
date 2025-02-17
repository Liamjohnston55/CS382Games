using UnityEngine;
using UnityEngine.SceneManagement;

public class startScreen : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
