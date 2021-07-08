using Managers;
using UnityEngine;

namespace UI
{
    public abstract class Element : MonoBehaviour
    {
        protected UIManager manager;
        
        private void Awake()
        {
            manager = ManagerLocator.Get<UIManager>();
            OnAwake();
        }

        protected virtual void OnAwake() {}
    }
}
