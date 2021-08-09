using E7.Minefield;

namespace Tests.Beacons
{
    public enum UIBeacons
    {
        AbilityA, AbilityB, AbilityC, AbilityD, EndTurn, Meditate
    }
    
    public class UIBeacon : NavigationBeacon<UIBeacons> {}
}
