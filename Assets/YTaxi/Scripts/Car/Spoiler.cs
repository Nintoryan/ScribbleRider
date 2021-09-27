using UnityEngine;

namespace YTaxi.Scripts.Car
{
    public class Spoiler : MonoBehaviour
    {
        [SerializeField] private Car _car;
        [SerializeField] private float _spoilerForce;
        private float _baseReductionCoef = 0.3f;
        [SerializeField] private Transform _point;
        
    
        public float _Coef
        {
            get;
            private set;
        }
    
        private void Start()
        {
            _Coef = _baseReductionCoef;
        }
    
        public void Disable()
        {
            _Coef = 0;
        }
    
        public void Reset()
        {
            _Coef = _baseReductionCoef;
        }
    
        public void FullPower()
        {
            _Coef = 1;
        }
    
        private void Update()
        {
            _car.Model.AddForceAtPosition(-_car.Model.transform.up*(_spoilerForce*_Coef),_point.position);
        }
    }
}

