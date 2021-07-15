using Tests.Beacons.Base;

namespace Tests.Beacons
{
    public enum GridBeacons
    {
        A, B, C, D, E, F, G, H, I, J, K, L, M
    }
    
    public class GridBeacon : AutoTileBeacon<GridBeacons> {}
}
