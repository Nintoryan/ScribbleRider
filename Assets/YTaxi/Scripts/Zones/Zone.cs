using UnityEngine;

namespace YTaxi.Zones
{
    public abstract class Zone : MonoBehaviour, IZone
    {
        public void OnTriggerStay(Collider other)
        {
            var carEffects = other.GetComponentInParent<CarEffects>();
            if (carEffects == null) { }
            else
            {
                AppyEffect(carEffects);
            }
            
        }
        public void OnTriggerExit(Collider other)
        {
            var carEffects = other.GetComponentInParent<CarEffects>();
            if (carEffects == null) return;
            DisposeEffect(carEffects);
        }
        public virtual void AppyEffect(CarEffects _carEffects) {}
        
        public virtual void DisposeEffect(CarEffects _carEffects) {}
    }
}
