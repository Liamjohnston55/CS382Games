using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ApplePicker.Apple
{
    public class AppleController 
    {
        private GameObject apple;
        private bool isFalling = true;
        private CancellationTokenSource cts;

        public GameObject stick;       // assign stick
        public float stickChance;      // % chance of a stick to spawn

        public AppleController(GameObject apple)
        {
            this.apple = apple;
            cts = new CancellationTokenSource();
            AppleFall();
        }

        async void AppleFall(){
            while (!cts.IsCancellationRequested) {
                await Task.Delay(1000, cts.Token);

                // stick spawn chance
                float random = Random.Range(0f, 100f);
                
                // This if statement will compare the random spawn chance to the set percentage of the level
                // if random is less than the stick chance then it will spawn a stick 
                if(random <= stickChance) {
                    // Spawn stick
                    GameObject.Instantiate(stick, new Vector3(Random.Range(165f, 264f), 35, -9), Quaternion.identity);
                }
                else {
                    // Spawn apple
                    GameObject.Instantiate(apple, new Vector3(Random.Range(165f, 264f), 35, -9), Quaternion.identity);
                }
            }
        }

        public void Disposed(){
            // Cancel the token to break out of the while loop.
            cts.Cancel();
        }
    }
}
