using UnityEngine;

public class SlowlyZone : MonoBehaviour
{
    public float _speedReduceCoef;
    private void OnTriggerStay(Collider other)
    {
        var car = other.GetComponentInParent<Car>();
        if (car != null)
        {
            car.ApplySlowEffect(_speedReduceCoef);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var car = other.GetComponentInParent<Car>();
        if (car != null)
        {
            car.DisposeSlowEffect();
        }
    }
}