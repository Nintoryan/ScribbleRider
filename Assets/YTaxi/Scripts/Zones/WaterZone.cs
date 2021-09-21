using UnityEngine;
using YTaxi.Scripts.Car;

namespace YTaxi.Scripts.Zones
{
    public class WaterZone : Zone
    {
        [SerializeField] private float _ArchimedCoef;
        private bool CanApplyEffect;
        private void FixedUpdate()
        {
            CanApplyEffect = true;
        }
        public override void AppyEffect(CarEffects _carEffects)
        {
            if(!CanApplyEffect) return;
            _carEffects.DisableSpoiler();
            _carEffects.ApplyForce(Physics.gravity*-_ArchimedCoef);
            CanApplyEffect = false;
        }
    
        public override void DisposeEffect(CarEffects _carEffects)
        {
            _carEffects.ResetSpoiler();
        }
    }
}

