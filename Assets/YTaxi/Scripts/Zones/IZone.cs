using UnityEngine;
using YTaxi;

public interface IZone
{
    void OnTriggerStay(Collider other);
    void OnTriggerExit(Collider other);
    void AppyEffect(CarEffects _carEffects);
    void DisposeEffect(CarEffects _carEffects);
}
