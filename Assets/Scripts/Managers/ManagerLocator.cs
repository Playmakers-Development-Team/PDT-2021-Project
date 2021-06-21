using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Managers
{
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

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<Type> types = new List<Type>();
            foreach (Assembly assembly in assemblies)
                types.AddRange(assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Manager))));
            
            foreach (Type type in types)
            {
                dynamic manager = Convert.ChangeType(Activator.CreateInstance(type), type);
                Register(manager);
            }
                
            foreach (var manager in Current.managers.Values)
                manager.ManagerStart();
        }
        
        public static T Get<T>() where T : Manager
        {
            string key = typeof(T).Name;
            
            if (!Current.managers.ContainsKey(key))
                throw new Exception($"{key} not registered with {Current.GetType()}.Name");

            return Current.managers[key] as T;
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
