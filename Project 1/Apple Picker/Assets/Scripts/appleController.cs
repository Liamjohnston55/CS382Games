using UnityEngine;

namespace ApplePicker.Apple
{

    public class appleController : MonoBehaviour 
    {
        public appleController() {
            // this is the constructor
            private GameObject apple;

            private appleController(GameObject apple) {
                this.apple = apple;
            }
        }

        async void AppleFall() {
            while(isFalling) {
                // This is so that way it does not crash 
                await Task.Delay(1000);
                // this will summon the apple to the object that we desire
                // random.range will randomly go inbetween the two specified values 
                GameObject.Instantiate(apple, new Vector3(Random.Range(-8f, 8f), 5f, 5f ), Quaternion.identity);
            }
        }

    }
}