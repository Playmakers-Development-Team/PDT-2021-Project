using E7.Minefield;

namespace Tests.Beacons
{
    public enum UIBeacons
    {
        EndTurn, Meditate
    }
    
    public class UIBeacon : NavigationBeacon<UIBeacons> {}
}
