using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.DI
{
    /// <summary>
    /// Simple service locator for dependency management.
    /// Provides a centralized registry for services, enabling loose coupling
    /// between components without a full DI framework.
    ///
    /// In a production project, consider replacing with VContainer, Zenject, or similar.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        /// <summary>
        /// Register a service instance for a given interface type.
        /// </summary>
        public static void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"[ServiceLocator] Overwriting existing service: {type.Name}");
            }
            _services[type] = service;
            Debug.Log($"[ServiceLocator] Registered: {type.Name}");
        }

        /// <summary>
        /// Resolve a service by its interface type.
        /// </summary>
        public static T Resolve<T>() where T : class
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var service))
            {
                return (T)service;
            }

            Debug.LogError($"[ServiceLocator] Service not found: {type.Name}");
            return null;
        }

        /// <summary>
        /// Try to resolve a service. Returns false if not registered.
        /// </summary>
        public static bool TryResolve<T>(out T service) where T : class
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var obj))
            {
                service = (T)obj;
                return true;
            }
            service = null;
            return false;
        }

        /// <summary>
        /// Check if a service is registered.
        /// </summary>
        public static bool IsRegistered<T>() where T : class
        {
            return _services.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Unregister a service.
        /// </summary>
        public static void Unregister<T>() where T : class
        {
            var type = typeof(T);
            if (_services.Remove(type))
            {
                Debug.Log($"[ServiceLocator] Unregistered: {type.Name}");
            }
        }

        /// <summary>
        /// Clear all registered services. Call on application quit or scene transitions.
        /// </summary>
        public static void Clear()
        {
            _services.Clear();
            Debug.Log("[ServiceLocator] All services cleared.");
        }
    }
}
