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
            string path = $"{DEFAULT_DIRECTORY}{fullFileName}";
            File.WriteAllText(path, data);

            // Refresh the Asset Database
            AssetDatabase.Refresh();

            string newFileName = fileName + mapIndex;
            _gridSpawner.SetActiveMap(newFileName);

            Debug.Log("[TACTIKIT/MapEditor] Map saved to Asset Database at: " + path);
        }

        private void SaveData(string data, string fileName = DEFAULT_FILE_NAME)
        {
            string fullFileName = $"{fileName}{DEFAULT_FILE_TYPE}";
            string path = $"{DEFAULT_DIRECTORY}{fullFileName}";
            File.WriteAllText(path, data);

            // Refresh the Asset Database
            AssetDatabase.Refresh();

            Debug.Log("[TACTIKIT/MapEditor] Map saved to Asset Database at: " + path);
        }
    }

    [System.Serializable]
    public class SerializationWrapper<T>
    {
        public T[] items;
    }
}