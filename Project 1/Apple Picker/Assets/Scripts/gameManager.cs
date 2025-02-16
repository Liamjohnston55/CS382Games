using UnityEngine;
using ApplePicker.Apple;

namespace ApplePicker
{
    public class GameManager : MonoBehaviour
    {
        public GameObject apple;

        private AppleController appleController;

        // Start is called before the first execution of Update after the MonoBehaviour is created
        void Start() {
            appleController = new AppleController(apple);
        }

        void OnDestroy() {
            appleController.Disposed();
        }
    }
}
