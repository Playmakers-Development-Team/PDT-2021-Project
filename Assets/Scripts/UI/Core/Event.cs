using UnityEngine.Events;

namespace UI.Core
{
    /// <summary>
    /// Generic encapsulated unity event for UI system usage.
    /// </summary>
    /// <typeparam name="T">Event parameter type.</typeparam>
    public class Event<T>
    {
        private readonly UnityEvent<T> unityEvent;
        
        
        internal Event() => unityEvent = new UnityEvent<T>();

        
        internal void Invoke(T argument) => unityEvent.Invoke(argument);
        
        internal void AddListener(UnityAction<T> action) => unityEvent.AddListener(action);

        internal void RemoveListener(UnityAction<T> action) => unityEvent.RemoveListener(action);
        
        internal void RemoveAllListeners() => unityEvent.RemoveAllListeners();
    }
    

    /// <summary>
    /// Encapsulated unity event for UI system usage.
    /// </summary>
    public class Event
    {
        private readonly UnityEvent unityEvent;

        
        internal Event() => unityEvent = new UnityEvent();

        
        internal void Invoke() => unityEvent.Invoke();
        
        internal void AddListener(UnityAction action) => unityEvent.AddListener(action);

        internal void RemoveListener(UnityAction action) => unityEvent.RemoveListener(action);

        internal void RemoveAllListeners() => unityEvent.RemoveAllListeners();
    }
}
