using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ApplePicker.Apple;
using TMPro;

namespace ApplePicker
{
    public class GameManager : MonoBehaviour
    {
        [Header("Apple Spawning")]
        public GameObject apple; // Assign your apple prefab here

        [Header("Round & UI Settings")]
        public TMP_Text roundCounterText;      // Assign in-Game
        public GameObject restartPanel;    // Asign in-Game
        public Button restartButton;       // Assign in-Game
        
        [Header("Round Settings")]
        public int currentRound = 1;       // Begin at the first level or "round"
        public int maxRound = 10;          // This is the final level 


        // This is the number of apples needed to advance to the next round
        private int applesRequired;
        // This is the current number of apples in the basket
        private int applesCaught;

        private AppleController appleController;

        void Start() {
            // Hides the reset button until you die
            if (restartPanel != null)
                restartPanel.SetActive(false);

            // initiate the current round
            StartRound(currentRound);
        }

        void StartRound(int round) {
            currentRound = round;
            applesCaught = 0;
            // Each round increases the amount of apples by 10
            // This function will be called every round
            applesRequired = 5 + (round - 1) * 10;

            // Update the round counter
            if (roundCounterText != null)
                roundCounterText.text = "Round " + currentRound;

            if (appleController != null)
                appleController.Disposed();

            // Begin spawning 
            appleController = new AppleController(apple);
        }

        public void AppleCaught() {
            applesCaught++;

            // checking to see if we have won or lost
            if (applesCaught >= applesRequired)
            {
                if (currentRound < maxRound) {
                    // you won this round!! now advance 
                    StartRound(currentRound + 1);
                }
                else {
                    // you have lost, go back to title screen
                    GameOver();
                }
            }
        }

        // This function preps unity for the switch to the title screen or scene 
        public void GameOver() {
            // Stopping apple spawns
            if (appleController != null) {
                appleController.Disposed();
            }

            // unhide the restart button
            if (restartPanel != null) {
                restartPanel.SetActive(true);
            }
        }

        // Reset button code
        // This will need to be put into the button "on click()" function to get us back to the title screen
        public void RestartGame()
        {
            SceneManager.LoadScene("StartMenu");
        }

        void OnDestroy() {
            if (appleController != null)
                appleController.Disposed();
        }
    }
}
