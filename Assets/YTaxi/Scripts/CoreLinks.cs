using UnityEngine;

namespace YTaxi.Scripts
{
    public class CoreLinks : MonoBehaviour
    {
        public static CoreLinks Instance;

        public Car.Car Player;
        public Car.Car Bot;

        private void Awake()
        {
            Instance = this;
        }
    }
}
