namespace TactiKit.MapEditor
{
#if UNITY_EDITOR
    using UnityEngine;
    using UnityEditor;
    using System.IO;
    using static TactiKit.MapEditor.Constants;

    /// <summary>
    /// A class for saving images of tile prefabs to be used as thumbnails in the UI.
    /// </summary>
    public class TileImageSaver : MonoBehaviour
    {
        private Vector3 _hiddenPosition = new Vector3(1000, 1000, 1000);

        [SerializeField] private Camera _renderCamera;
        [SerializeField] private float _cameraDistance = 2f;
        [SerializeField] private float _cameraFOV = 30f;
        [SerializeField] private float _yOffset = 10f;

        private void Start()
        {
            if (_renderCamera == null)
            {
                Debug.LogError("[TACTIKIT/MapEditor] Tile Render Camera is not assigned.");
                return;
            }
        }

        /// <summary>
        /// Captures and re-saves all tiles at once.
        /// </summary>
        /// <param name="targetDirectory">The target directory to save the tile output.</param>
        public void CaptureAndSaveAllTiles(string targetDirectory = null)
        {
            GameObject[] tilePrefabs = Resources.LoadAll<GameObject>(TILE_RESOURCES_FOLDER);

            foreach (GameObject prefab in tilePrefabs)
            {
                CaptureAndSaveTile(prefab, targetDirectory);
            }
        }

        /// <summary>
        /// Captures and re-saves a single tile.
        /// </summary>
        /// <param name="tilePrefab">The tile to be saved.</param>
        /// <param name="targetDirectory">The target directory to save the tile output.</param>
        public void CaptureAndSaveTile(GameObject tilePrefab, string targetDirectory = null)
        {
            GameObject instance = InstantiateTilePrefab(tilePrefab);

            PositionAndAimCamera(instance);

            _renderCamera.Render();
            Texture2D texture = CaptureTileImage(_renderCamera.targetTexture);

            if (string.IsNullOrEmpty(targetDirectory))
            {
                SaveTextureAsPNG(texture, tilePrefab.name);
            }
            else
            {
                SaveTextureAsPNG(texture, tilePrefab.name, targetDirectory);
            }

            DestroyImmediate(instance);
        }

        /// <summary>
        /// Captures a tile image as a PNG.
        /// </summary>
        /// <param name="renderTexture">The renderTexture of the originally rendered tile.</param>
        /// <returns></returns>
        public byte[] CaptureTileImageAsPNG(RenderTexture renderTexture)
        {
            Texture2D texture = CaptureTileImage(renderTexture);
            return texture.EncodeToPNG();
        }

        private void SaveTextureAsPNG(Texture2D texture, string prefabName, string targetDirectory = null)
        {
            string folderPath = targetDirectory ?? TILE_TEXTURES_DIRECTORY;
            string pngPath = Path.Combine(folderPath, $"{prefabName}.png");

            byte[] pngData = texture.EncodeToPNG();
            Directory.CreateDirectory(folderPath);
            File.WriteAllBytes(pngPath, pngData);
            Debug.Log($"[TACTIKIT/MapEditor] Saved PNG file: {pngPath}");
        }

        private GameObject InstantiateTilePrefab(GameObject prefab)
        {
            Quaternion spawnRotation = Quaternion.identity;
            return Instantiate(prefab, _hiddenPosition, spawnRotation);
        }

        private void PositionAndAimCamera(GameObject target)
        {
            _renderCamera.transform.position = _hiddenPosition + new Vector3(0, _cameraDistance, -_cameraDistance);
            Vector3 lookAtPoint = target.transform.position + Vector3.up * _yOffset;
            _renderCamera.transform.LookAt(lookAtPoint);
        }

        private Texture2D CaptureTileImage(RenderTexture renderTexture)
        {
            RenderTexture.active = renderTexture;
            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();
            RenderTexture.active = null;
            return texture;
        }

        private void SaveTextureAsAsset(Texture2D texture, string prefabName)
        {
            string assetPath = $"{TILE_TEXTURES_DIRECTORY}{prefabName}{ASSET_FILE_TYPE}";
            AssetDatabase.CreateAsset(texture, assetPath);
        }
    }
#endif
}