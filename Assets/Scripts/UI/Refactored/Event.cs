using UnityEngine.Events;

namespace UI.Refactored
{
    public class Event<T>
    {
        private readonly UnityEvent<T> unityEvent;
        
        
        public Event()
        {
            unityEvent = new UnityEvent<T>();
        }

        
        public void AddListener(UnityAction<T> action) => unityEvent.AddListener(action);

        public void RemoveListener(UnityAction<T> action) => unityEvent.RemoveListener(action);

        
        internal void Invoke(T argument) => unityEvent.Invoke(argument);
        
        internal void RemoveAllListeners() => unityEvent.RemoveAllListeners();
    }
    

    public class Event
    {
        private readonly UnityEvent unityEvent;

        
        public Event()
        {
            unityEvent = new UnityEvent();
        }

        
        public void AddListener(UnityAction action) => unityEvent.AddListener(action);

        public void RemoveListener(UnityAction action) => unityEvent.RemoveListener(action);
        
        
        internal void Invoke() => unityEvent.Invoke();

        internal void RemoveAllListeners() => unityEvent.RemoveAllListeners();
    }
}
