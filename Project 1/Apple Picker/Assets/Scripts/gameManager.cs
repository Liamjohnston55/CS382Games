using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ApplePicker.Apple;
using TMPro;
using System.Collections.Generic;
using System.Linq;

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

        // This is the variable that keeps track of the baskets
        private List<BasketController> baskets = new List<BasketController>();

        // This function makes sure that there is only one game manager
        // multiple game managers will reset the lives every time
        void Awake(){
            var managers = FindObjectsOfType<GameManager>();

            if (managers.Length > 1) {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject); // DO NOT REMOVE BREAKS EVERYTHING
        }

        void Start() {
            //print("GameManager lives: " + lives);

            // Find all the baskets and store them here
            // using a .ToList() so i can use commands like .remove and .add because its easy
            baskets = FindObjectsOfType<BasketController>().ToList();

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

            // Remove basket when one life is lost
            // Only remove a basket if we have more than 1
            // removing the last one messes with the controller script
            if (baskets.Count > 1) {
                BasketController highestBasket = baskets[0];

                // foreach will go through each element of the list of baskets
                // find the "top" basket and remove it
                foreach (BasketController basket in baskets) {
                    if (basket.transform.position.y > highestBasket.transform.position.y) {
                        highestBasket = basket;
                    }
                }

                // Remove the basket from the list
                baskets.Remove(highestBasket);

                // Remove the basket from the hierarchy 
                Destroy(highestBasket.gameObject);
                print("[DEBUG] Removed highest basket at Z: " + highestBasket.transform.position.y);
            } 
            else {
                print("[DEBUG] Last basket remaining, not deleting.");
            }

            //print("Number of lives left " + lives);
            if (lives == 0) {
                //print("Game over has been called to from health decrease");
                GameOver();
            }
        }

        public void AppleCaught() {
            applesCaught++;
            
            if(applesCaught == 35) {
                print("you won!!");
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
