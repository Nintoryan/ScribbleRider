using UnityEngine;

public class CarEffects : MonoBehaviour
{
    [SerializeField] private Car _car;
    
    public void ApplySlowEffect(float _speedReduceCoef)
    {
        _car.WheelSpeed = Mathf.Lerp(_car.BaseWheelSpeed * _speedReduceCoef, 1, _car.NonlinearityСoeff / 100f);
        _car.ModelSpeed = Mathf.Lerp(_car.BaseModelSpeed * _speedReduceCoef, 1, _car.NonlinearityСoeff / 100f);
    }
    public void DisposeSlowEffect()
    {
        _car.WheelSpeed = _car.BaseWheelSpeed;
        _car.ModelSpeed = _car.BaseModelSpeed;
    }

    public void ApplyIceEffect()
    {
        switch (_car.AmountOfSharpAngles)
        {
            case 0:
                _car.ModelSpeed = _car.BaseModelSpeed / 10f;
                break;
            case 1:
                _car.ModelSpeed = _car.BaseModelSpeed / 3f;
                break;
            default:
                _car.ModelSpeed = _car.BaseModelSpeed / 1.5f;
                break;
        }
    }

    public void DisposeIceEffect()
    {
        _car.ModelSpeed = _car.BaseModelSpeed;
    }

    public void ApplyForce(Vector3 _force)
    {
        _car.Model.AddForce(_force*_car.Model.mass);
    }
}
