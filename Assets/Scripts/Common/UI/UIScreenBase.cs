using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Common.UI
{
    /// <summary>
    /// Base class for all UI screens using UI Toolkit.
    /// Provides common functionality for UI document management, element
    /// initialization, and proper callback lifecycle management.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public abstract class UIScreenBase : MonoBehaviour
    {
        [Header("UI Document")]
        [SerializeField] protected UIDocument uiDocument;

        protected VisualElement root;

        // Store registered callbacks for proper cleanup
        private readonly Dictionary<Button, EventCallback<ClickEvent>> _registeredCallbacks
            = new Dictionary<Button, EventCallback<ClickEvent>>();

        protected virtual void Awake()
        {
            if (uiDocument == null)
                uiDocument = GetComponent<UIDocument>();
        }

        protected virtual void OnEnable()
        {
            if (uiDocument == null)
            {
                Debug.LogError($"[{GetType().Name}] UIDocument component is missing!");
                return;
            }

            root = uiDocument.rootVisualElement;

            if (root == null)
            {
                Debug.LogError($"[{GetType().Name}] Root visual element is null!");
                return;
            }

            InitializeUIElements();
            RegisterCallbacks();
            OnScreenEnabled();
        }

        protected virtual void OnDisable()
        {
            UnregisterCallbacks();
            UnregisterAllButtonCallbacks();
            OnScreenDisabled();
        }

        /// <summary>
        /// Initialize UI elements by querying the root visual element.
        /// Override this to cache references to UI elements.
        /// </summary>
        protected abstract void InitializeUIElements();

        /// <summary>
        /// Register button callbacks and event listeners.
        /// Override this to set up event handlers.
        /// </summary>
        protected virtual void RegisterCallbacks() { }

        /// <summary>
        /// Unregister button callbacks and event listeners.
        /// Override this to clean up event handlers.
        /// </summary>
        protected virtual void UnregisterCallbacks() { }

        /// <summary>
        /// Called after UI elements are initialized and callbacks are registered.
        /// Override this for screen-specific initialization logic.
        /// </summary>
        protected virtual void OnScreenEnabled() { }

        /// <summary>
        /// Called before callbacks are unregistered.
        /// Override this for screen-specific cleanup logic.
        /// </summary>
        protected virtual void OnScreenDisabled() { }

        /// <summary>
        /// Helper method to safely query UI elements with error logging.
        /// </summary>
        protected T QueryElement<T>(string elementName) where T : VisualElement
        {
            var element = root.Q<T>(elementName);
            if (element == null)
            {
                Debug.LogWarning($"[{GetType().Name}] UI element '{elementName}' of type {typeof(T).Name} not found");
            }
            return element;
        }

        /// <summary>
        /// Helper method to register button click callbacks.
        /// Stores the callback reference for proper cleanup on disable.
        /// </summary>
        protected void RegisterButtonCallback(Button button, System.Action callback)
        {
            if (button == null || callback == null) return;

            // Create and store the callback wrapper so we can unregister the same instance
            EventCallback<ClickEvent> handler = evt => callback();
            button.RegisterCallback(handler);
            _registeredCallbacks[button] = handler;
        }

        /// <summary>
        /// Unregister a specific button callback.
        /// </summary>
        protected void UnregisterButtonCallback(Button button)
        {
            if (button == null) return;

            if (_registeredCallbacks.TryGetValue(button, out var handler))
            {
                button.UnregisterCallback(handler);
                _registeredCallbacks.Remove(button);
            }
        }

        /// <summary>
        /// Unregister all stored button callbacks.
        /// Called automatically on OnDisable.
        /// </summary>
        private void UnregisterAllButtonCallbacks()
        {
            foreach (var kvp in _registeredCallbacks)
            {
                if (kvp.Key != null)
                {
                    kvp.Key.UnregisterCallback(kvp.Value);
                }
            }
            _registeredCallbacks.Clear();
        }
    }
}
