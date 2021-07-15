using System;
using Managers;
using UnityEngine;

namespace UI
{
    [Obsolete("Element is obsolete, inherit from UIComponent<T> instead.")]
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
