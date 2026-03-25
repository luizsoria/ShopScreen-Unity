using UnityEngine;
using Common.DI;
using Common.Events;
using Common.UI;

namespace Shop
{
    /// <summary>
    /// Bootstrap script for the Shop scene.
    /// Initializes the navigation controller and sets up the initial state.
    /// Ensures clean service and event state on scene load/unload.
    /// Attach this to a GameObject in the ShopScene.
    /// </summary>
    public class ShopSceneBootstrap : MonoBehaviour
    {
        private void Start()
        {
            // Clear any previous state
            UINavigationController.Clear();
            EventBus.Clear();
            ServiceLocator.Clear();

            Debug.Log("[ShopSceneBootstrap] Shop scene initialized.");
        }

        private void OnDestroy()
        {
            UINavigationController.Clear();
            EventBus.Clear();
            ServiceLocator.Clear();
        }
    }
}
