using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Units;
using Units.Commands;

namespace UI
{
    [Serializable]
    public class UIManager : Manager
    {
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

        internal T GetDialogue<T>() where T : Dialogue => dialogues.Find(d => d is T) as T;
    }
}
