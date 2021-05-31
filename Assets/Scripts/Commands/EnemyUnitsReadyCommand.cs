using Units;

namespace Commands
{
    public class EnemyUnitsReadyCommand : Command
    {
        // TODO Don't actually need unit to be a parameter, for now just pass in null
        public EnemyUnitsReadyCommand(IUnit unit) : base(unit) {}

        public override void Queue() {}

        public override void Execute() {}

        public override void Undo() {}
    }
}