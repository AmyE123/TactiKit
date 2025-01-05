namespace TactiKit.MapEditor
{
    using System.IO;
    using UnityEngine;
    using static TactiKit.MapEditor.Constants;

    public class GridSpawner : MonoBehaviour
    {
        private int _width = DEFAULT_GRID_SIZE;
        private int _height = DEFAULT_GRID_SIZE;

        private float _minX, _maxX, _minZ, _maxZ;

        private bool _mapSpawned = false;

        [SerializeField] private Vector3 _centerOfGrid;
        [SerializeField] private GameObject _defaultTerrainPrefab;
        [SerializeField] private GameObject _gridParent;
        [SerializeField] private GridSerializer _serializer;
        [SerializeField] private string _activeMapName;

        [SerializeField] private CameraManager _cameraManager;

        public string ActiveMapName => _activeMapName;
        public bool MapSpawned => _mapSpawned;

        public Vector3 CenterOfGrid => _centerOfGrid;

        private void Start()
        {
            _mapSpawned = false;
            InitializeBounds();
            CheckProperties();
        }

        private void InitializeBounds()
        {
            _minX = _minZ = 0;
            _maxX = _maxZ = 0;
        }

        private void CheckProperties()
        {
            if (_gridParent == null)
            {
                Debug.LogError("[TACTIKIT/MapEditor] Grid Parent is not assigned!");
            }

            if (_serializer == null)
            {
                Debug.LogError("[TACTIKIT/MapEditor] Grid Serializer is not assigned!");
            }

            if (_defaultTerrainPrefab == null)
            {
                Debug.LogError("[TACTIKIT/MapEditor] Default Terrain Prefab is not assigned!");
            }
        }

        private void SpawnGridTiles(GameObject terrainType)
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Vector3 position = new Vector3(x, 0, y);
                    GameObject tile = Instantiate(terrainType, position, Quaternion.identity);
                    tile.transform.SetParent(_gridParent.transform);

                    // Update grid bounds
                    _minX = Mathf.Min(_minX, position.x);
                    _maxX = Mathf.Max(_maxX, position.x);
                    _minZ = Mathf.Min(_minZ, position.z);
                    _maxZ = Mathf.Max(_maxZ, position.z);
                }
            }
        }

        private void SpawnGridTilesFromFile(string gridName)
        {
            string json = File.ReadAllText($"{DEFAULT_MAPS_DIRECTORY}{gridName}{DEFAULT_FILE_TYPE}");
            SerializationWrapper<TileObjectData> data = JsonUtility.FromJson<SerializationWrapper<TileObjectData>>(json);

            foreach (TileObjectData tileObjectData in data.items)
            {
                // Assuming you have a method to instantiate prefabs by name
                GameObject prefab = GetPrefabByName(tileObjectData.PrefabName);
                if (prefab == null)
                {
                    Debug.LogError("[TACTIKIT/MapEditor] Prefab in data not found. Have you renamed any tiles? \n Replacing null tile with default.");
                    prefab = _defaultTerrainPrefab;
                }

                Vector3 position = new Vector3(tileObjectData.Position[0], tileObjectData.Position[1], tileObjectData.Position[2]);
                var tile = Instantiate(prefab, position, Quaternion.identity);
                tile.transform.SetParent(_gridParent.transform);

                // Update grid bounds
                _minX = Mathf.Min(_minX, position.x);
                _maxX = Mathf.Max(_maxX, position.x);
                _minZ = Mathf.Min(_minZ, position.z);
                _maxZ = Mathf.Max(_maxZ, position.z);
            }
        }

        private void ClearGrid()
        {
            for (int i = _gridParent.transform.childCount - 1; i >= 0; i--)
            {
                GameObject child = _gridParent.transform.GetChild(i).gameObject;
                Destroy(child);
            }
        }

        private GameObject GetPrefabByName(string prefabName)
        {
            // Load all GameObjects in the "Tiles" folder and its subfolders
            GameObject[] allPrefabs = Resources.LoadAll<GameObject>("Tiles");

            // Search for the prefab with the specified name
            foreach (GameObject prefab in allPrefabs)
            {
                if (prefab.name == prefabName)
                {
                    return prefab;
                }
            }
            Debug.LogError("[TACTIKIT/MapEditor] Prefab not found " + prefabName);
            return null;
        }

        /// <summary>
        /// Sets the active map value.
        /// </summary>
        /// <param name="mapName">The name of the map you want to load.</param>
        public void SetActiveMap(string mapName)
        {
            _activeMapName = mapName;
        }

        /// <summary>
        /// Loads a new map with the inputted terrain type.
        /// </summary>
        /// <param name="terrainType">The terrain to fill the map with.</param>
        public void LoadNewMap(GameObject terrainType)
        {
            _mapSpawned = true;

            ClearGrid();
            SpawnGridTiles(terrainType);
            AssignTilesToGridPositions();
            PositionCamera();
        }

        /// <summary>
        /// Loads a grid from file name.
        /// </summary>
        /// <param name="mapName">The name of the map you want to load.</param>
        public void LoadMapFromFile(string mapName)
        {
            _mapSpawned = true;

            if (mapName.EndsWith(DEFAULT_FILE_TYPE))
            {
                mapName = mapName.Replace(".json", "");
            }

            ClearGrid();
            SpawnGridTilesFromFile(mapName);
            AssignTilesToGridPositions();
            PositionCamera();
        }

        /// <summary>
        /// Loads the currently active map.
        /// </summary>
        public void LoadActiveMap()
        {
            _mapSpawned = true;

            LoadMapFromFile(_activeMapName);
        }

        /// <summary>
        /// Sets and assigns the grid positions of all the tiles.   
        /// </summary>
        public void AssignTilesToGridPositions()
        {
            Vector3 gridOrigin = Vector3.zero;
            float tileSize = DEFAULT_TILE_SIZE;

            for (int i = 0; i < _gridParent.transform.childCount; i++)
            {
                GameObject child = _gridParent.transform.GetChild(i).gameObject;

                // Calculate grid coordinates based on world position
                Vector3 relativePos = child.transform.position - gridOrigin;
                int x = Mathf.FloorToInt(relativePos.x / tileSize);
                int z = Mathf.FloorToInt(relativePos.z / tileSize);

                if (x >= 0 && x < _serializer.Grid.GetLength(0) && z >= 0 && z < _serializer.Grid.GetLength(1))
                {
                    _serializer.Grid[x, z] = child;
                }
            }
        }

        /// <summary>
        /// Positions the camera to the center of the newly spawned map.
        /// </summary>
        public void PositionCamera()
        {
            float gridWidth = _maxX - _minX;
            float gridHeight = _maxZ - _minZ;
            Vector3 centerPosition = new Vector3(_minX + gridWidth / 2, 10, _minZ + gridHeight / 2);
            _centerOfGrid = centerPosition;

            _cameraManager.ResetCameras();          
        }
    }
}