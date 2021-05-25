using System;
using System.Collections.Generic;
using Commands.Shapes;
using GridObjects;
using Managers;
using Units;
using UnityEngine;

namespace Commands
{
    public class AbilityCommand : Command
    {
        private IShape shape;
        private int damage;
        private int knockback;
        private Vector2 targetVector;

        private GridManager gridManager;

        public AbilityCommand(IUnit unit, Vector2 targetVector, IShape shape, int damage, int knockback) : base(unit)
        {
            this.shape = shape;
            this.damage = damage;
            this.knockback = knockback;
            this.targetVector = targetVector;

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
            foreach (IUnit targetUnit in shape.GetTargets(unit.Coordinate, targetVector))
            {
                Vector2Int coordinate = targetUnit.Coordinate;
                List<GridObject> gridObjects = gridManager.GetGridObjectsByCoordinate(coordinate);

                foreach (var gridObject in gridObjects)
                {
                    action(gridObject);
                }
            }
        }
    }
}
