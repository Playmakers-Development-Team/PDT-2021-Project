using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public interface IManager {}
    
    public class ManagerLocator
    {
        private readonly Dictionary<string, Manager> managers = new Dictionary<string, Manager>();
        
        public static ManagerLocator Current { get; private set; }
        
        private ManagerLocator()
        {
            Debug.Log($"Initialized ManagerLocator.");
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            Current = new ManagerLocator();
            
            // REGISTER SERVICES HERE
            Register(new CommandManager());
            Register(new PlayerManager());
            Register(new TurnManager());
            Register(new GridManager());
            
            foreach (var manager in Current.managers.Values)
            {
                manager.ManagerStart();
            }
        }
        
        public static T Get<T>() where T : Manager
        {
            string key = typeof(T).Name;
            
            if (!Current.managers.ContainsKey(key))
                throw new Exception($"{key} not registered with {Current.GetType()}.Name");

            return (T) Current.managers[key];
        }

        public static void Register<T>(T service) where T : Manager
        {
            string key = typeof(T).Name;
            
            if (Current.managers.ContainsKey(key))
            {
                Debug.Log($"Attempted to register service of type {key} which is already registered with the {Current.GetType().Name}.");
                return;
            }

            Debug.Log($"Registered {key}!");
            
            Current.managers.Add(key, service);
        }

        public static void Unregister<T>() where T : Manager
        {
            string key = typeof(T).Name;
            
            if (!Current.managers.ContainsKey(key))
            {
                Debug.Log($"Attempted to unregister service of type {key} which is not registered with the {Current.GetType().Name}.");
                return;
            }

            Current.managers.Remove(key);
        }
    }
}
