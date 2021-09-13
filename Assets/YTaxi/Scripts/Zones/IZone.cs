using UnityEngine;
using YTaxi;

namespace YTaxi.Zones
{
    public interface IZone
    {
        void OnTriggerStay(Collider other);
        void OnTriggerExit(Collider other);
        void AppyEffect(CarEffects _carEffects);
        void DisposeEffect(CarEffects _carEffects);
    }

}
