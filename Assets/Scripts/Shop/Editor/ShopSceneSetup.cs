#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;

namespace Shop.Editor
{
    /// <summary>
    /// Editor utility to help set up the Shop scene with all required components.
    /// Access via menu: Tools > Shop > Setup Shop Scene.
    /// Creates a new scene, configures all GameObjects, saves it, and adds it to Build Settings.
    /// </summary>
    public static class ShopSceneSetup
    {
        private const string SCENE_PATH = "Assets/Scenes/ShopScene.unity";

        [MenuItem("Tools/Shop/Setup Shop Scene")]
        public static void SetupScene()
        {
            // Create a new empty scene
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

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

            // Ensure Scenes directory exists
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }

            // Save the scene
            EditorSceneManager.SaveScene(scene, SCENE_PATH);

            // Add scene to Build Settings if not already there
            AddSceneToBuildSettings(SCENE_PATH);

            Debug.Log("[ShopSceneSetup] Shop scene setup complete! " +
                      "Scene saved to " + SCENE_PATH + " and added to Build Settings.\n" +
                      "If you haven't created the catalog yet, run Tools > Shop > Create Default Catalog.");

            Selection.activeGameObject = shopScreenGO;
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(
                EditorBuildSettings.scenes);

            // Check if scene is already in build settings
            foreach (var s in scenes)
            {
                if (s.path == scenePath) return;
            }

            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
#endif
