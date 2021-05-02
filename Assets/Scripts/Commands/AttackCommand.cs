using GridObjects;
using Units;

namespace Commands
{
    public class AttackCommand : Command
    {
        private GridObject target;
        private int damage;
        private int knockback;

        public AttackCommand(IUnit unit, GridObject target, int damage, int knockback) : base(unit)
        {
            this.target = target;
            this.damage = damage;
            this.knockback = knockback;
        }

        public override void Queue() {}

        public override void Execute()
        {
            target.TakeDamage(damage);
            target.TakeKnockback(knockback);
        }

        public override void Undo()
        {
            target.TakeDamage(-damage);
            target.TakeKnockback(-knockback);
        }
    }
}
