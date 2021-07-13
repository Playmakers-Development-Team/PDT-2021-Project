using Managers;
using UnityEngine;

namespace UI
{
    public abstract class Element : MonoBehaviour
    {
        protected UIManager manager;
        
        protected void Awake()
        {
            manager = ManagerLocator.Get<UIManager>();
            OnComponentAwake();
        }

        protected virtual void OnComponentAwake() {}
    }
}
