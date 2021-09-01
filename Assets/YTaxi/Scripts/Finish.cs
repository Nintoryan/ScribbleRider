using UnityEngine;
using YTaxi;

public class Finish : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var carEffects = other.GetComponentInParent<CarEffects>();
        if (carEffects != null)
        {
            carEffects.ModelSpeed *= 0;
            carEffects.WheelSpeed *= 0;
        }
    }
}
