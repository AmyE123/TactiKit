namespace TactiKit.MapEditor
{
    using UnityEngine;
    using System.IO;
    using static TactiKit.MapEditor.Constants;

    /// <summary>
    /// A UI class for loading map buttons in the load UI.
    /// </summary>
    public class UI_LoadButtonsLoader : MonoBehaviour
    {
        [SerializeField] private UIManager _uiManager;

        [SerializeField] private GameObject _mapButtonPrefab;
        [SerializeField] private Transform _mapContentArea;

        /// <summary>
        /// Refresh the map buttons list incase a new map has been added.
        /// </summary>
        public void RefreshMapButtons()
        {
            LoadMapButtons(_mapContentArea, _mapButtonPrefab);
        }

        private void Start()
        {
            LoadMapButtons(_mapContentArea, _mapButtonPrefab);

            if(_uiManager == null)
            {
                _uiManager = FindObjectOfType<UIManager>();
            }
        }

        private void LoadMapButtons(Transform contentParent, GameObject buttonPrefab)
        {
            ClearContent(contentParent);

            string mapsFolderPath;

#if UNITY_EDITOR
            mapsFolderPath = DEFAULT_MAPS_DIRECTORY;
#else
            string buildFolderPath = Path.GetDirectoryName(Application.dataPath);
            mapsFolderPath = Path.Combine(buildFolderPath, DEFAULT_MAPS_DIRECTORY); // Use build folder in standalone
#endif

            string[] mapFiles = Directory.GetFiles(mapsFolderPath, "*.json");

            foreach (string filePath in mapFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string lastAccessed = File.GetLastAccessTime(filePath).ToString("yyyy-MM-dd HH:mm:ss");

                GameObject button = CreateMapButton(contentParent, buttonPrefab, fileName, lastAccessed, filePath);
                button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => SelectMap(filePath));
            }
        }

        private GameObject CreateMapButton(Transform contentParent, GameObject buttonPrefab, string mapName, string lastAccessed, string filePath)
        {
            GameObject button = Instantiate(buttonPrefab, contentParent);
            MapButtonManager mapButtonManager = button.GetComponent<MapButtonManager>();
            mapButtonManager.SetMapData(mapName, lastAccessed, filePath);

            return button;
        }

        private void ClearContent(Transform contentParent)
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
        }

        private void SelectMap(string filePath)
        {
            _uiManager.SelectMapFromFiles(filePath);
        }

    }

}