using UnityEngine;

namespace YTaxi.Zones
{
    public class OutOfBounds : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var carEffects = other.GetComponentInParent<CarEffects>();
            if (carEffects != null)
            {
                carEffects.InvokeOutOfBounds();
            }
        }
    }

}
