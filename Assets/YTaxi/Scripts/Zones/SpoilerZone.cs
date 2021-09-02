using YTaxi;

public class SpoilerZone : Zone
{
    public override void AppyEffect(CarEffects _carEffects)
    {
        _carEffects.EnableSpoiler();
    }

    public override void DisposeEffect(CarEffects _carEffects)
    {
        _carEffects.DisableSpoiler();
    }
}