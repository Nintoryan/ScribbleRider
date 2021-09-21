using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using YTaxi.Scripts.Bot;
using YTaxi.Scripts.Car.Wheels;

namespace YTaxi.Scripts.Car
{
    public class Car : MonoBehaviour
    {
        [SerializeField] private List<Transform> _wheelPoints;

        [SerializeField] private Rigidbody _model;

        [SerializeField] private float _wheelsSpeed;
        [SerializeField] private float _modelSpeed;
        public event UnityAction OnFirstWheelSet;
        public event UnityAction OnOutOfBounds;
        public event UnityAction OnFinished;
        private bool _finished;
        
        public Wheel _currentWheel { get; private set; }
        
        public float WheelSpeed
        {
            get => _wheelsSpeed;
            set => _wheelsSpeed = value;
        }
        public float ModelSpeed
        {
            get => _modelSpeed;
            set => _modelSpeed = value;
        }

        public float WheelSpeedCoef = 1f;
        public float ModelSpeedCoef = 1f;

        public Rigidbody Model => _model;
        public float BaseWheelSpeed { get; private set; }
        public float BaseModelSpeed { get; private set; }

        private readonly List<GameObject> _currentWheels = new List<GameObject>();

        private void Start()
        {
            BaseWheelSpeed = _wheelsSpeed;
            BaseModelSpeed = _modelSpeed;
        }

        public void ApplyModifyedBaseSpeed(BotSpeedModifyer _modifyer)
        {
            WheelSpeedCoef  =  _modifyer.SpeedCoef;
            ModelSpeedCoef = _modifyer.SpeedCoef;
        }
        
        private void FixedUpdate()
        {
            if(_finished) return;
            foreach (var wheel in _currentWheels)
            {
                wheel.GetComponent<Rigidbody>().AddTorque(new Vector3(0, 0, -1) * (_wheelsSpeed * WheelSpeedCoef));
            }
            var forward = _model.transform.forward;
            var Velocity = new Vector3(forward.x, Mathf.Clamp(forward.y, 0, 100000), forward.z);
            Velocity = Vector3.Lerp(Velocity, _model.transform.up, 0.05f);
            if (!Mathf.Approximately(_modelSpeed, 0))
                _model.velocity = Velocity * (_modelSpeed * ModelSpeedCoef);
        }
        

        public void Finish()
        {
            if (!_finished)
            {
                _finished = true;
                OnFinished?.Invoke();
            }
        }

        public void SetWheels(Wheel _wheel)
        {
            _currentWheel = _wheel;
            IESetWheels(_wheel);
        }

        private void IESetWheels(Wheel wheelExample)
        {
            foreach (var wheel in _currentWheels)
            {
                Destroy(wheel);
            }

            _currentWheels.Clear();
            
            foreach (var t in _wheelPoints)
            {
                var wheel = Instantiate(wheelExample.gameObject, transform);
                wheel.transform.position = t.position;
                wheel.transform.localScale *= 0.005f;
                wheel.transform.SetParent(transform);
                var joint = wheel.AddComponent<HingeJoint>();
                joint.anchor = Vector3.zero;
                joint.connectedBody = _model;
                joint.axis = new Vector3(0, 0, -1);
                joint.massScale = 2;
                joint.connectedMassScale = 1000f;
                _currentWheels.Add(wheel);
            }

            Destroy(wheelExample.gameObject);
            if (_model.isKinematic)
            {
                OnFirstWheelSet?.Invoke();
                _model.isKinematic = false;
            }
        }

        public void InvokeOutOfBounds()
        {
            OnOutOfBounds?.Invoke();
        }
    }

}
