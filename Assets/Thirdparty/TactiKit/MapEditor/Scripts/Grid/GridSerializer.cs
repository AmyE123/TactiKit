namespace TactiKit.MapEditor
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using static TactiKit.MapEditor.Constants;

    public class GridSerializer : MonoBehaviour
    {
        private GameObject[,] _grid = new GameObject[DEFAULT_GRID_SIZE, DEFAULT_GRID_SIZE];

        [SerializeField] private GridSpawner _gridSpawner;

        public GameObject[,] Grid => _grid;

        public bool SaveGrid(int gridIndex, string fileName = DEFAULT_FILE_NAME)
        {
            _gridSpawner.AssignTilesToGridPositions();
            var gridData = ExtractGridData();

            if (gridData.Count == 0)
            {
                Debug.LogWarning("[TACTIKIT/MapEditor] No GameObjects found in the grid to serialize.");
                return false;
            }

            SerializeAndSave(gridData, gridIndex, fileName);
            return true;
        }

        public bool SaveGrid(string fileName = DEFAULT_FILE_NAME)
        {
            _gridSpawner.AssignTilesToGridPositions();
            var gridData = ExtractGridData();

            if (gridData.Count == 0)
            {
                Debug.LogWarning("[TACTIKIT/MapEditor] No GameObjects found in the grid to serialize.");
                return false;
            }

            SerializeAndSave(gridData, fileName);
            return true;
        }

        private List<TileObjectData> ExtractGridData()
        {
            List<TileObjectData> gridData = new List<TileObjectData>();

            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                for (int y = 0; y < _grid.GetLength(1); y++)
                {
                    GameObject gridObject = _grid[x, y];
                    if (gridObject != null)
                    {
                        gridData.Add(new TileObjectData(gridObject));
                    }
                }
            }

            return gridData;
        }

        private void SerializeAndSave(List<TileObjectData> gridData, int gridIndex, string fileName = DEFAULT_FILE_NAME)
        {
            SerializationWrapper<TileObjectData> dataWrapper = new SerializationWrapper<TileObjectData> { items = gridData.ToArray() };
            string json = JsonUtility.ToJson(dataWrapper);

            SaveData(json, gridIndex, fileName);
        }

        private void SerializeAndSave(List<TileObjectData> gridData, string fileName = DEFAULT_FILE_NAME)
        {
            SerializationWrapper<TileObjectData> dataWrapper = new SerializationWrapper<TileObjectData> { items = gridData.ToArray() };
            string json = JsonUtility.ToJson(dataWrapper);

            SaveData(json, fileName);
        }

        private void SaveData(string data, int mapIndex, string fileName = DEFAULT_FILE_NAME)
        {
            string fullFileName = $"{fileName}{mapIndex}{DEFAULT_FILE_TYPE}";
            string path = GetSavePath(fullFileName);

            File.WriteAllText(path, data);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif // UNITY_EDITOR

            string newFileName = fileName + mapIndex;
            _gridSpawner.SetActiveMap(newFileName);

            Debug.Log($"[TACTIKIT/MapEditor] Map saved to: {path}");
        }

        private void SaveData(string data, string fileName = DEFAULT_FILE_NAME)
        {
            string fullFileName = $"{fileName}{DEFAULT_FILE_TYPE}";
            string path = GetSavePath(fullFileName);

            File.WriteAllText(path, data);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif // UNITY_EDITOR

            Debug.Log($"[TACTIKIT/MapEditor] Map saved to: {path}");
        }

        private string GetSavePath(string fileName)
        {
#if UNITY_EDITOR
            string directoryPath = DEFAULT_MAPS_DIRECTORY;
#else
            string buildFolderPath = Path.GetDirectoryName(Application.dataPath);
            string directoryPath = Path.Combine(buildFolderPath, DEFAULT_MAPS_DIRECTORY);
#endif // UNITY_EDITOR

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            return Path.Combine(directoryPath, fileName);
        }
    }

    [System.Serializable]
    public class SerializationWrapper<T>
    {
        public T[] items;
    }
}