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

        public AppleController(GameObject apple)
        {
            this.apple = apple;
            cts = new CancellationTokenSource();
            AppleFall();
        }

        async void AppleFall(){
            while (!cts.IsCancellationRequested) {
                await Task.Delay(1000, cts.Token);
                GameObject.Instantiate(apple, new Vector3(Random.Range(165f, 265f), 35, -9), Quaternion.identity);
            }
        }

        public void Disposed(){
            // Cancel the token to break out of the while loop.
            cts.Cancel();
        }
    }
}
