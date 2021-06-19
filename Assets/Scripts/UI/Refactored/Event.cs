using UnityEngine.Events;

namespace UI.Refactored
{
    public class Event<T>
    {
        private readonly UnityEvent<T> unityEvent;
        
        
        internal Event() => unityEvent = new UnityEvent<T>();

        
        internal void AddListener(UnityAction<T> action) => unityEvent.AddListener(action);

        internal void RemoveListener(UnityAction<T> action) => unityEvent.RemoveListener(action);

        
        internal void Invoke(T argument) => unityEvent.Invoke(argument);
        
        internal void RemoveAllListeners() => unityEvent.RemoveAllListeners();
    }
    

    public class Event
    {
        private readonly UnityEvent unityEvent;

        
        internal Event() => unityEvent = new UnityEvent();

        
        internal void AddListener(UnityAction action) => unityEvent.AddListener(action);

        internal void RemoveListener(UnityAction action) => unityEvent.RemoveListener(action);
        
        
        internal void Invoke() => unityEvent.Invoke();

        internal void RemoveAllListeners() => unityEvent.RemoveAllListeners();
    }
}
