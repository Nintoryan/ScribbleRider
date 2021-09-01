using YTaxi;

public class SlowlyZone : Zone
{
    public float _speedReduceCoef;

    public override void AppyEffect(CarEffects _carEffects)
    {
        _carEffects.ApplySlowEffect(_speedReduceCoef);
    }

    public override void DisposeEffect(CarEffects _carEffects)
    {
        _carEffects.DisposeSlowEffect();
    }
}