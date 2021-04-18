using Units;

namespace Commands
{
    public class AttackCommand : Command
    {
        public AttackCommand(IUnit unit) : base(unit) {}

        public override void Queue() {}

        public override void Execute() {}

        public override void Undo() {}

        void Start() {}

        void Update() {}
    }
}
