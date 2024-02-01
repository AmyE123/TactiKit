namespace TactiKit.MapEditor
{
#if UNITY_EDITOR
    using UnityEngine;    
    using System.Collections;
    using UnityEditor;
    using static TactiKit.MapEditor.Constants;

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

        private void RefreshAllTiles()
        {
            ConfigureCamera();
            StartCoroutine(CaptureAndSaveAllTilesCoroutine(_renderCamera.targetTexture));
        }

        private void ConfigureCamera()
        {
            _renderCamera.clearFlags = CameraClearFlags.SolidColor;
            _renderCamera.backgroundColor = new Color(0, 0, 0, 0);
            _renderCamera.fieldOfView = _cameraFOV;
        }

        public void CaptureAndSaveAllTiles()
        {
            GameObject[] tilePrefabs = Resources.LoadAll<GameObject>(TILE_RESOURCES_FOLDER);

            foreach (GameObject prefab in tilePrefabs)
            {
                CaptureAndSaveTile(prefab);
            }
        }

        public void CaptureAndSaveTile(GameObject tilePrefab)
        {
            GameObject instance = InstantiateTilePrefab(tilePrefab);

            PositionAndAimCamera(instance);

            // Manually render the camera and capture the image
            _renderCamera.Render();
            CaptureTileImage(_renderCamera.targetTexture);

            CaptureAndProcessTileImage(_renderCamera.targetTexture, tilePrefab.name);
            DestroyImmediate(instance);
        }

        private IEnumerator CaptureAndSaveAllTilesCoroutine(RenderTexture renderTexture)
        {
            GameObject[] tilePrefabs = Resources.LoadAll<GameObject>(TILE_RESOURCES_FOLDER);

            foreach (GameObject prefab in tilePrefabs)
            {
                yield return StartCoroutine(CaptureAndSaveTileCoroutine(renderTexture, prefab));
            }
        }

        private IEnumerator CaptureAndSaveTileCoroutine(RenderTexture renderTexture, GameObject tilePrefab)
        {
            GameObject instance = InstantiateTilePrefab(tilePrefab);

            PositionAndAimCamera(instance);

            yield return new WaitForEndOfFrame();

            CaptureAndProcessTileImage(renderTexture, tilePrefab.name);

            DestroyImmediate(instance);
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

        private void CaptureAndProcessTileImage(RenderTexture renderTexture, string prefabName)
        {
            Texture2D texture = CaptureTileImage(renderTexture);
            SaveTextureAsAsset(texture, prefabName);
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