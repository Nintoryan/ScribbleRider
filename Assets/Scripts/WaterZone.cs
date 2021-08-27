using UnityEngine;

public class WaterZone : MonoBehaviour
{
    [SerializeField] private float _ArchimedCoef;

    public bool CanApplyEffect
    {
        get;
        private set;
    }

    private void FixedUpdate()
    {
        CanApplyEffect = true;
    }

    private void OnTriggerStay(Collider other)
    {
        var car = other.GetComponentInParent<Car>();
        if (car != null && CanApplyEffect)
        {
            car.Model.AddForce(Physics.gravity*car.Model.mass*-_ArchimedCoef);
            CanApplyEffect = false;
        }
    }
}
