using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Events
{
    /// <summary>
    /// Lightweight event bus for decoupled communication between UI components.
    /// Follows the Observer pattern with type-safe event channels.
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, Delegate> _handlers = new Dictionary<Type, Delegate>();

        /// <summary>
        /// Subscribe to an event type.
        /// </summary>
        public static void Subscribe<T>(Action<T> handler) where T : struct
        {
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var existing))
            {
                _handlers[type] = Delegate.Combine(existing, handler);
            }
            else
            {
                _handlers[type] = handler;
            }
        }

        /// <summary>
        /// Unsubscribe from an event type.
        /// </summary>
        public static void Unsubscribe<T>(Action<T> handler) where T : struct
        {
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var existing))
            {
                var result = Delegate.Remove(existing, handler);
                if (result == null)
                    _handlers.Remove(type);
                else
                    _handlers[type] = result;
            }
        }

        /// <summary>
        /// Publish an event to all subscribers.
        /// </summary>
        public static void Publish<T>(T eventData) where T : struct
        {
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var existing))
            {
                ((Action<T>)existing)?.Invoke(eventData);
            }
        }

        /// <summary>
        /// Clear all subscriptions. Call on scene unload to prevent leaks.
        /// </summary>
        public static void Clear()
        {
            _handlers.Clear();
            Debug.Log("[EventBus] All subscriptions cleared.");
        }
    }
}
