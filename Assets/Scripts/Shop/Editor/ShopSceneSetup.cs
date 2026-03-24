#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Shop.Editor
{
    /// <summary>
    /// Editor utility to help set up the Shop scene with all required components.
    /// Access via menu: Tools > Shop > Setup Shop Scene.
    /// </summary>
    public static class ShopSceneSetup
    {
        [MenuItem("Tools/Shop/Setup Shop Scene")]
        public static void SetupScene()
        {
            // Create Bootstrap GameObject
            var bootstrapGO = new GameObject("ShopSceneBootstrap");
            bootstrapGO.AddComponent<ShopSceneBootstrap>();

            // Create ShopScreen GameObject
            var shopScreenGO = new GameObject("ShopScreen");
            var uiDocument = shopScreenGO.AddComponent<UIDocument>();

            // Try to find and assign the UXML asset
            string[] guids = AssetDatabase.FindAssets("ShopScreen t:VisualTreeAsset");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
                uiDocument.visualTreeAsset = visualTree;
            }

            // Try to find and assign PanelSettings
            string[] panelGuids = AssetDatabase.FindAssets("ShopPanelSettings t:PanelSettings");
            if (panelGuids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(panelGuids[0]);
                var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(path);
                uiDocument.panelSettings = panelSettings;
            }

            var shopScreen = shopScreenGO.AddComponent<Shop.UI.ShopScreen>();

            // Try to find and assign ShopCatalog
            string[] catalogGuids = AssetDatabase.FindAssets("ShopCatalog t:ScriptableObject");
            if (catalogGuids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(catalogGuids[0]);
                var catalog = AssetDatabase.LoadAssetAtPath<Shop.Data.ShopCatalog>(path);
                if (catalog != null)
                {
                    var serializedObj = new SerializedObject(shopScreen);
                    var catalogProp = serializedObj.FindProperty("shopCatalog");
                    if (catalogProp != null)
                    {
                        catalogProp.objectReferenceValue = catalog;
                        serializedObj.ApplyModifiedProperties();
                    }
                }
            }

            Debug.Log("[ShopSceneSetup] Shop scene setup complete! " +
                      "Remember to assign the ShopCatalog ScriptableObject to the ShopScreen component.");

            Selection.activeGameObject = shopScreenGO;
        }
    }
}
#endif
