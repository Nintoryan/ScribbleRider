using YTaxi;

public class SpoilerZone : Zone
{
    public override void AppyEffect(CarEffects _carEffects)
    {
        _carEffects.FullPowerSpoiler();
    }

    public override void DisposeEffect(CarEffects _carEffects)
    {
        _carEffects.ResetSpoiler();
    }
}