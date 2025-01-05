#if UNITY_EDITOR
namespace TactiKit.MapEditor
{
    using UnityEditor;
    using UnityEngine;
    using System.IO;

    /// <summary>
    /// A class for all tile image saving editor functions.
    /// </summary>
    public class TileImageSaverEditor
    {
        [MenuItem("Tools/TactiKit/MapEditor/Tiles/Update All Tile Images")]
        private static void RefreshAllTileTextures()
        {
            TileImageSaver tileImgSaver = Object.FindObjectOfType<TileImageSaver>();
            if (tileImgSaver == null)
            {
                Debug.LogError("[TACTIKIT/MapEditor] TileImageSaver not found in the scene.");
                return;
            }

            tileImgSaver.CaptureAndSaveAllTiles();
        }

        [MenuItem("Tools/TactiKit/MapEditor/Tiles/Update Tile Image")]
        private static void OpenTileImageSaverPopup()
        {
            TileImageSaverPopup.ShowWindow();
        }

        [MenuItem("Tools/TactiKit/MapEditor/Tiles/Update Tiles From Directory")]
        private static void UpdateTilesFromDirectory()
        {
            string directory = EditorUtility.OpenFolderPanel("Select Directory for Tile Images", "", "");
            if (string.IsNullOrEmpty(directory))
            {
                Debug.LogWarning("[TACTIKIT/MapEditor] No directory selected.");
                return;
            }

            TileImageSaver tileImgSaver = Object.FindObjectOfType<TileImageSaver>();
            if (tileImgSaver == null)
            {
                Debug.LogError("[TACTIKIT/MapEditor] TileImageSaver not found in the scene.");
                return;
            }

            tileImgSaver.CaptureAndSaveAllTiles(directory);
            Debug.Log($"[TACTIKIT/MapEditor] Updated all tile images in directory: {directory}");
        }
    }

    /// <summary>
    /// A class for the tile saver editor window popup for updating individual tile images.
    /// </summary>
    public class TileImageSaverPopup : EditorWindow
    {
        private GameObject[] tilePrefabs;
        private TileImageSaver tileImgSaver;
        private Vector2 scrollPosition;

        /// <summary>
        /// Shows the window for the tile image saver tool.
        /// </summary>
        public static void ShowWindow()
        {
            var window = GetWindow<TileImageSaverPopup>("Update Tile Images");
            window.Show();
        }

        private void OnEnable()
        {
            tileImgSaver = FindObjectOfType<TileImageSaver>();
            tilePrefabs = Resources.LoadAll<GameObject>(Constants.TILE_RESOURCES_FOLDER);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Update Tile Image", EditorStyles.boldLabel);

            if (tileImgSaver == null)
            {
                EditorGUILayout.HelpBox("TileImageSaver component not found in the scene. Please add it to a GameObject.", MessageType.Error);
                if (GUILayout.Button("Refresh"))
                {
                    tileImgSaver = FindObjectOfType<TileImageSaver>();
                }
                return;
            }

            if (tilePrefabs == null || tilePrefabs.Length == 0)
            {
                EditorGUILayout.HelpBox("No tile prefabs found. Ensure tiles are located in the specified resources folder.", MessageType.Warning);
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (var prefab in tilePrefabs)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(prefab.name, GUILayout.Width(200));

                if (GUILayout.Button("Update", GUILayout.Width(100)))
                {
                    UpdateTileImage(prefab);
                }

                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        private void UpdateTileImage(GameObject tilePrefab)
        {
            if (tileImgSaver != null)
            {
                tileImgSaver.CaptureAndSaveTile(tilePrefab);
                Debug.Log($"[TACTIKIT/MapEditor] Updated image for tile: {tilePrefab.name}");
            }
            else
            {
                Debug.LogError("[TACTIKIT/MapEditor] TileImageSaver not found.");
            }
        }
    }
}
#endif // UNITY_EDITOR