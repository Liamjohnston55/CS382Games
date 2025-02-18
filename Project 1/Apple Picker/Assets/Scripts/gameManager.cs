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
        public GameObject stick; // Assign your stick prefab here

        [Header("Round & UI Settings")]
        public TMP_Text roundCounterText;      // Assign in-Game
        public GameObject restartPanel;        // Asign in-Game
        public Button restartButton;           // Assign in-Game
        
        [Header("Round Settings")]
        public int currentRound = 1;       // Begin at the first level or "round" (Assign In-Game)
        public int maxRound = 5;          // This is the final level             (Assign In-Game)

        int lives = 4;  // lives before you die 


        // This is the number of apples needed to advance to the next round
        private int applesRequired;
        // This is the current number of apples in the basket
        private int applesCaught;

        private AppleController appleController;

        // Debugging 
        void Awake(){
            print("GameManager Awake: " + gameObject.name);
            var managers = FindObjectsOfType<GameManager>();
            print("[DEBUG] Number of GameManager instances: " + managers.Length);

            if (managers.Length > 1) {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }
        // Debugging 

        void Start() {
            print("GameManager lives: " + lives);
            print($"GameManager Start - Lives: {lives} on {gameObject.name}");

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

            // stop spawning (apple)
            if (appleController != null)
                appleController.Disposed();

            // Begin spawning (apple)
            appleController = new AppleController(apple);
    
            // Stick spawn chance (based upon the current round)
            if(appleController != null) {
                appleController.stick = stick;
                appleController.stickChance = 10f + (currentRound - 1f) * (20f / 9f);
            }   
        }

        public void HealthDecrease() {
            print("You lost a life!");
            lives = lives - 1;
            print("Number of lives left " + lives);
            if (lives == 0) {
                print("Game over has been called to from health decrease");
                GameOver();
            }
        }

        public void AppleCaught() {
            applesCaught++;

            print($"[DEBUG] Apple Caught - currentRound: {currentRound}, applesCaught: {applesCaught}, applesRequired: {applesRequired}");
            
            if(applesCaught == 35) {
                print("you won!!");
                print("movingn to the winning scene");
                SceneManager.LoadScene("WinnerScene");
            }
            
            // checking to see if we have won or lost
            if (applesCaught >= applesRequired) {
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
        public void RestartGame() {
            Destroy(gameObject);
            SceneManager.LoadScene("StartMenu");
        }

        void OnDestroy() {
            if (appleController != null)
                appleController.Disposed();
        }
    }
}
