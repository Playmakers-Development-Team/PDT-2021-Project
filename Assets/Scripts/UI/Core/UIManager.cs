using System;
using System.Collections.Generic;
using System.Linq;
using Managers;

namespace UI.Core
{
    /// <summary>
    /// A singleton manager responsible for controlling the <see cref="Dialogue"/> 'stack'.
    /// </summary>
    [Serializable]
    public class UIManager : Manager
    {
        internal readonly Event<Dialogue> dialogueAdded = new Event<Dialogue>();
        private readonly List<Dialogue> dialogues = new List<Dialogue>();

        /// <summary>
        /// Add a <see cref="Dialogue"/> to the top of the stack.
        /// </summary>
        /// <param name="dialogue">The <see cref="Dialogue"/> to be added.</param>
        /// <typeparam name="T">The type of <see cref="Dialogue"/>.</typeparam>
        internal void Add<T>(T dialogue) where T : Dialogue
        {
            Dialogue front = dialogues.FirstOrDefault();
            if (front != null)
                front.Demote();

            dialogues.Insert(0, dialogue);
            dialogueAdded.Invoke(dialogue);

            dialogue.Promote();
        }

        /// <summary>
        /// Remove a <see cref="Dialogue"/> from the stack.
        /// </summary>
        /// <param name="dialogue">The <see cref="Dialogue"/> to remove.</param>
        internal void Remove(Dialogue dialogue)
        {
            dialogues.Remove(dialogue);
            dialogue.Close();
        }

        /// <summary>
        /// Remove the currently active <see cref="Dialogue"/> from the top of the stack.
        /// </summary>
        /// <returns>Returns the <see cref="Dialogue"/> that was removed.</returns>
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

        /// <summary>
        /// Queries the manager for the currently active <see cref="Dialogue"/> without altering the stack.
        /// </summary>
        /// <returns>Returns the currently active <see cref="Dialogue"/>.</returns>
        internal Dialogue Peek() => dialogues.FirstOrDefault();

        /// <summary>
        /// Queries the manager for the highest instance of a <see cref="Dialogue"/> of type <c>T</c> in the stack.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="Dialogue"/> to search for.</typeparam>
        /// <returns>Returns the first instance of <c>T</c> in the stack, starting from the top.</returns>
        internal T GetDialogue<T>() where T : Dialogue => dialogues.Find(d => d.GetType().IsAssignableFrom(typeof(T))) as T;
    }
}
