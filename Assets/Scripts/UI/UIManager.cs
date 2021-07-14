using System;
using System.Collections.Generic;
using System.Linq;
using Abilities;
using Commands;
using Managers;
using Turn.Commands;
using Units;
using Units.Commands;
using UnityEngine;

namespace UI
{
    [Serializable]
    public class UIManager : Manager
    {
        #region Refactoring

        internal readonly Event<Dialogue> dialogueAdded = new Event<Dialogue>();
        private readonly List<Dialogue> dialogues = new List<Dialogue>();

        internal void Add<T>(T dialogue) where T : Dialogue
        {
            Dialogue front = dialogues.FirstOrDefault();
            if (front != null)
                front.Demote();

            dialogues.Insert(0, dialogue);
            dialogueAdded.Invoke(dialogue);

            dialogue.Promote();
        }

        internal void Remove(Dialogue dialogue)
        {
            dialogues.Remove(dialogue);
            dialogue.Hide();
        }

        internal Dialogue Pop()
        {
            Dialogue removed = Peek();

            if (removed != null)
            {
                dialogues.Remove(removed);
                removed.Hide();
            }

            Dialogue front = dialogues.FirstOrDefault();
            if (front != null)
                front.Promote();

            return removed;
        }

        internal Dialogue Peek() => dialogues.FirstOrDefault();

        #endregion


        // Unit
        internal readonly Event<IUnit> unitSelected = new Event<IUnit>();
        internal readonly Event unitDeselected = new Event();

        internal readonly Event<StatDifference> unitDamaged = new Event<StatDifference>();

        // Abilities
        internal readonly Event<Ability> abilitySelected = new Event<Ability>();
        internal readonly Event<Ability> abilityDeselected = new Event<Ability>();

        internal readonly Event<Vector2> abilityRotated = new Event<Vector2>();

        internal readonly Event abilityConfirmed = new Event();

        // Turn
        // TODO: Should have type struct that contains information of the previous and current units, rounds, etc.
        internal readonly Event turnChanged = new Event();


        public override void ManagerStart()
        {
            CommandManager cmd = ManagerLocator.Get<CommandManager>();

            cmd.ListenCommand((StartTurnCommand c) => turnChanged.Invoke());
            cmd.ListenCommand((EndTurnCommand c) => turnChanged.Invoke());
            cmd.ListenCommand((TurnQueueCreatedCommand c) => turnChanged.Invoke());
            cmd.ListenCommand((TakeTotalDamageCommand c) => unitDamaged.Invoke(new StatDifference(c)));

            // TODO: TEMPORARY...
            turnChanged.AddListener(unitDeselected.Invoke);
        }
    }


    internal readonly struct StatDifference
    {
        internal IUnit Unit { get; }
        internal int NewValue { get; }
        internal int OldValue { get; }
        internal int BaseValue { get; }
        internal int Difference { get; }

        internal StatDifference(TakeTotalDamageCommand cmd)
        {
            // TODO: Update so that Difference is positive when stat going up, negative when going down..
            // TODO: Have this constructor simply call the other...
            Unit = cmd.Unit;
            NewValue = Unit.Health.HealthPoints.Value;
            OldValue = NewValue + cmd.Value;
            BaseValue = Unit.Health.HealthPoints.BaseValue;
            Difference = OldValue - NewValue;
        }

        internal StatDifference(IUnit unit, int newValue, int oldValue, int baseValue)
        {
            Unit = unit;
            NewValue = newValue;
            OldValue = oldValue;
            BaseValue = baseValue;
            Difference = oldValue - newValue;
        }
    }
}
