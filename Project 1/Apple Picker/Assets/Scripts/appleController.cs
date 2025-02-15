using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ApplePicker.Apple
{
    public class AppleController : MonoBehaviour
    {
        public GameObject apple;
        private bool isFalling = true;
        private CancellationTokenSource cts;

        // NOTE: Unity won't call this constructor automatically. 
        // Typically, you would not use a constructor with parameters in a MonoBehaviour.
        public AppleController(GameObject apple)
        {
            this.apple = apple;
            this.cts = new CancellationTokenSource();
        }

        private async void AppleFall()
        {
            while (!cts.IsCancellationRequested)
            {
                // Wait 1 second before spawning another apple.
                await Task.Delay(1000, cts.Token);
                
                // Instantiate an apple at a random x-position and fixed y,z.
                Instantiate(
                    apple, 
                    new Vector3(Random.Range(-8f, 8f), 5f, 0f), 
                    Quaternion.identity
                );
            }
        }

        public void Dispose()
        {
            // Cancel the token to break out of the while loop.
            cts.Cancel();
        }
    }
}
