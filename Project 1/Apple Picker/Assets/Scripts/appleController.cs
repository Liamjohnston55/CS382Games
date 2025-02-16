using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ApplePicker.Apple
{
    public class AppleController 
    {
        private GameObject apple;
        private bool isFalling = true;

        public AppleController(GameObject apple)
        {
            this.apple = apple;
            AppleFall();
        }

        async void AppleFall(){
            while (isFalling) {
                await Task.Delay(1000);
                GameObject.Instantiate(apple, new Vector3(Random.Range(50f, -50f), 5, 0), Quaternion.identity);
            }
        }

        public void Disposed(){
            // Cancel the token to break out of the while loop.
            isFalling = false;
        }
    }
}
