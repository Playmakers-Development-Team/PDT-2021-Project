using Units;

namespace Playtest
{
    public class PlaytestUnitData
    {
        public PlaytestUnitData(IUnit unit, bool initialUnit)
        {
            Unit = unit;
            InitialUnit = initialUnit;
        }

        public IUnit Unit { get; }
        public int TimesMoved { get; set; }
        public int DistanceMoved { get; set; }
        public bool InitialUnit { get; set; }
        public int TimesTurnManipulated { get; set; }
    }
}
