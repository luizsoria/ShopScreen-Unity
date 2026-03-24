using UnityEngine;
using Common.UI;

namespace Shop
{
    /// <summary>
    /// Bootstrap script for the Shop scene.
    /// Initializes the navigation controller and sets up the initial state.
    /// Attach this to a GameObject in the ShopScene.
    /// </summary>
    public class ShopSceneBootstrap : MonoBehaviour
    {
        private void Start()
        {
            // Clear any previous navigation state
            UINavigationController.Clear();

            // The ShopScreen will register itself via OnScreenEnabled
            Debug.Log("[ShopSceneBootstrap] Shop scene initialized.");
        }

        private void OnDestroy()
        {
            UINavigationController.Clear();
        }
    }
}
