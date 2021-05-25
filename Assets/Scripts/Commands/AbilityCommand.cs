using System;
using System.Collections.Generic;
using Commands.Shapes;
using GridObjects;
using Managers;
using Units;

namespace Commands
{
    public class AbilityCommand : Command
    {
        private Shape shape;
        private int damage;
        private int knockback;

        private GridManager gridManager;

        public AbilityCommand(IUnit unit, Shape shape, int damage, int knockback) : base(unit)
        {
            this.shape = shape;
            this.damage = damage;
            this.knockback = knockback;

            gridManager = ManagerLocator.Get<GridManager>();
        }

        public override void Queue() {}

        public override void Execute()
        {
            ForEachTarget(gridObject =>
            {
                gridObject.TakeDamage((int) unit.DealDamageModifier.Modify(damage));
                gridObject.TakeKnockback(knockback);
            });
        }

        public override void Undo()
        {
            ForEachTarget(gridObject =>
            {
                gridObject.TakeDamage((int) unit.DealDamageModifier.Modify(damage) * -1);
                gridObject.TakeKnockback(knockback * -1);
            });
        }

        private void ForEachTarget(Action<GridObject> action)
        {
            foreach (var cell in shape.Cells)
            {
                List<GridObject> gridObjects = gridManager.GetGridObjectsByCoordinate(cell);

                foreach (var gridObject in gridObjects)
                {
                    action(gridObject);
                }
            }
        }
    }
}
