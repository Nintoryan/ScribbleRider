public class IceZone : Zone
{
    public override void AppyEffect(CarEffects _carEffects)
    {
        _carEffects.ApplyIceEffect();
    }

    public override void DisposeEffect(CarEffects _carEffects)
    {
        _carEffects.DisposeSlowEffect();
    }
}
