using TMPro;
using UnityEngine;

namespace YTaxi
{
    public class CarEffects : MonoBehaviour
    {
        [SerializeField] private Car _car;
        [SerializeField] private Spoiler _spoiler;
        public Car Car => _car; 
        
        public float WheelSpeed
        {
            get => _car.WheelSpeed;
            set => _car.WheelSpeed = value;
        }

        public float ModelSpeed
        {
            get => _car.ModelSpeed;
            set => _car.ModelSpeed = value;
        }

        public Rigidbody Model => _car.Model;
        

        public void ApplySlowEffect(float _speedReduceCoef)
        {
            _car.WheelSpeed = Mathf.Lerp(_car.BaseWheelSpeed * _speedReduceCoef, 1, _car._currentWheel._nonlinnearCoef / 100f);
            _car.ModelSpeed = Mathf.Lerp(_car.BaseModelSpeed * _speedReduceCoef, 1, _car._currentWheel._nonlinnearCoef / 100f);
        }
        public void ResetSpeed()
        {
            _car.WheelSpeed = _car.BaseWheelSpeed;
            _car.ModelSpeed = _car.BaseModelSpeed;
        }

        public void ApplyIceEffect()
        {
            switch (_car._currentWheel._amountOfSharpAngles)
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

        public void EnableSpoiler()
        {
            _spoiler.Enable = true;
        }

        public void DisableSpoiler()
        {
            _spoiler.Enable = false;
        }
    }
}

