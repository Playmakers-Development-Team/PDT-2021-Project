using System;
using System.Collections.Generic;
using System.Linq;
using Managers;

namespace UI.Core
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
            dialogue.Close();
        }

        internal Dialogue Pop()
        {
            Dialogue removed = Peek();

            if (removed != null)
            {
                dialogues.Remove(removed);
                removed.Close();
            }

            Dialogue front = dialogues.FirstOrDefault();
            if (front != null)
                front.Promote();

            return removed;
        }

        internal Dialogue Peek() => dialogues.FirstOrDefault();

        internal T GetDialogue<T>() where T : Dialogue => dialogues.Find(d => d.GetType().IsAssignableFrom(typeof(T))) as T;
    }
}
