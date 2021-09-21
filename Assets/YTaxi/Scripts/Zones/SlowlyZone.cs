using YTaxi.Scripts.Car;

namespace YTaxi.Scripts.Zones
{
    public class SlowlyZone : Zone
    {
        public float _speedReduceCoef;
    
        public override void AppyEffect(CarEffects _carEffects)
        {
            _carEffects.ApplySlowEffect(_speedReduceCoef);
        }
    
        public override void DisposeEffect(CarEffects _carEffects)
        {
            _carEffects.ResetSpeed();
        }
    }
}
