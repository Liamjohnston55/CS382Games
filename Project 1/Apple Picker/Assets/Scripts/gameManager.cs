using UnityEngine;

namespace ApplePicker 
{

    public class gameManager : MonoBehaviour 
    {
        // Put your apple object into this box in unity, this will apply the code to it
        public GameObject apple;

        void Start() {
            // watch vid again
            new appleController(apple);
            
            // this will keep calling our function every 1 seconds
            // that was it will spawn continuous apples 
            InvokeRepeating(nameof(AppleFall), 1f, 1f);
            // the "nameof" function will give us an error if we mess up applefall (not hardcoded)
        }

        void AppleFall() {
            // this will summon the apple to the object that we desire
            // random.range will randomly go inbetween the two specified values 
            Instantiate(apple, new Vector3(Random.Range(-8f, 8f), 5f, 5f ), Quaternion.identity);
        }

        void Update() {

        }

    }
}