#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;

namespace Shop.Editor
{
    /// <summary>
    /// Editor utility to set up the Shop scene with all required components.
    /// Access via menu: Tools > Shop > Setup Shop Scene.
    /// Creates PanelSettings (with default theme), scene, GameObjects, and adds to Build Settings.
    /// </summary>
    public static class ShopSceneSetup
    {
        private const string SCENE_PATH = "Assets/Scenes/ShopScene.unity";
        private const string PANEL_SETTINGS_PATH = "Assets/UI/ShopPanelSettings.asset";

        [MenuItem("Tools/Shop/Setup Shop Scene")]
        public static void SetupScene()
        {
            // Step 1: Create PanelSettings with default Unity theme
            var panelSettings = CreatePanelSettings();

            // Step 2: Create a new scene
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // Step 3: Create Bootstrap GameObject
            var bootstrapGO = new GameObject("ShopSceneBootstrap");
            bootstrapGO.AddComponent<ShopSceneBootstrap>();

            // Step 4: Create ShopScreen GameObject with UIDocument
            var shopScreenGO = new GameObject("ShopScreen");
            var uiDocument = shopScreenGO.AddComponent<UIDocument>();

            // Assign PanelSettings
            uiDocument.panelSettings = panelSettings;

            // Find and assign the UXML asset
            string[] guids = AssetDatabase.FindAssets("ShopScreen t:VisualTreeAsset");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
                uiDocument.visualTreeAsset = visualTree;
                Debug.Log($"[ShopSceneSetup] Assigned VisualTreeAsset: {path}");
            }
            else
            {
                Debug.LogWarning("[ShopSceneSetup] Could not find ShopScreen.uxml. " +
                    "Please assign it manually to the UIDocument component.");
            }

            // Add ShopScreen component
            var shopScreen = shopScreenGO.AddComponent<Shop.UI.ShopScreen>();

            // Find and assign ShopCatalog
            string[] catalogGuids = AssetDatabase.FindAssets("ShopCatalog t:ShopCatalog");
            if (catalogGuids.Length == 0)
            {
                // Fallback: search by ScriptableObject type
                catalogGuids = AssetDatabase.FindAssets("ShopCatalog t:ScriptableObject");
            }

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
                        Debug.Log($"[ShopSceneSetup] Assigned ShopCatalog: {path}");
                    }
                }
            }
            else
            {
                Debug.LogWarning("[ShopSceneSetup] ShopCatalog not found. " +
                    "Please run 'Tools > Shop > Create Default Catalog' first, then run this setup again.");
            }

            // Step 5: Ensure Scenes directory exists and save scene
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }

            EditorSceneManager.SaveScene(scene, SCENE_PATH);

            // Step 6: Add scene to Build Settings
            AddSceneToBuildSettings(SCENE_PATH);

            Debug.Log("[ShopSceneSetup] Setup complete!\n" +
                      $"  - PanelSettings: {PANEL_SETTINGS_PATH}\n" +
                      $"  - Scene: {SCENE_PATH}\n" +
                      "  - Scene added to Build Settings\n" +
                      "  Press Play to test the shop UI.");

            Selection.activeGameObject = shopScreenGO;
        }

        private static PanelSettings CreatePanelSettings()
        {
            // Ensure UI directory exists
            if (!AssetDatabase.IsValidFolder("Assets/UI"))
            {
                AssetDatabase.CreateFolder("Assets", "UI");
            }

            // Check if PanelSettings already exists
            var existing = AssetDatabase.LoadAssetAtPath<PanelSettings>(PANEL_SETTINGS_PATH);
            if (existing != null)
            {
                Debug.Log("[ShopSceneSetup] PanelSettings already exists, reusing.");
                return existing;
            }

            // Create new PanelSettings
            var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();

            // Configure for landscape 1920x1080
            panelSettings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            panelSettings.referenceResolution = new Vector2Int(1920, 1080);
            panelSettings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
            panelSettings.match = 0.5f;

            // Assign the default Unity runtime theme
            var defaultTheme = FindDefaultTheme();
            if (defaultTheme != null)
            {
                panelSettings.themeStyleSheet = defaultTheme;
                Debug.Log("[ShopSceneSetup] Assigned default Unity theme to PanelSettings.");
            }
            else
            {
                Debug.LogWarning("[ShopSceneSetup] Could not find default Unity theme. " +
                    "UI may not render properly. Please assign a Theme Style Sheet manually " +
                    "in the PanelSettings inspector.");
            }

            AssetDatabase.CreateAsset(panelSettings, PANEL_SETTINGS_PATH);
            AssetDatabase.SaveAssets();

            Debug.Log($"[ShopSceneSetup] Created PanelSettings at {PANEL_SETTINGS_PATH}");
            return panelSettings;
        }

        private static ThemeStyleSheet FindDefaultTheme()
        {
            // Search for the default Unity runtime theme
            string[] themeGuids = AssetDatabase.FindAssets("UnityDefaultRuntimeTheme t:ThemeStyleSheet");
            if (themeGuids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(themeGuids[0]);
                return AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(path);
            }

            // Fallback: search for any ThemeStyleSheet
            themeGuids = AssetDatabase.FindAssets("t:ThemeStyleSheet");
            if (themeGuids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(themeGuids[0]);
                return AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(path);
            }

            return null;
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
