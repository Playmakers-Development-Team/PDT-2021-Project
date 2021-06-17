using System;
using Managers;
using UnityEngine;

namespace UI.Refactored
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

        protected virtual void Refresh() {}
    }
}
