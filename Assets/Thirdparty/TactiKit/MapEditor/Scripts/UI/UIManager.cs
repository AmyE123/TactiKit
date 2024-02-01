namespace TactiKit.MapEditor
{
    using System.IO;
    using TMPro;
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.UI;

    public class UIManager : MonoBehaviour
    {
        private bool _hasSelectedMap = false;
        private bool _hasSavedChanges = false;

        #region Create New Map Private Params

        private bool _iterativeSaving = false;
        private bool _customDirectory = false;

        #endregion // Create New Map Private Params

        [SerializeField] private TMP_Text _fileNameText;
        [SerializeField] private GridSerializer _gridSerializer;
        [SerializeField] private GridSpawner _gridSpawner;
        [SerializeField] private ToolboxManager _toolboxManager;
        [SerializeField] private ToolboxTileSelectionHandler _toolboxTileSelectionHandler;

        [Header("Popup Managers")]
        [SerializeField] private PopupManager CreateNewMapPopupManager;
        [SerializeField] private PopupManager LoadMapPopupManager;
        [SerializeField] private PopupManager SaveAsPopupManager;

        [Header("Load Map Parameters")]
        [SerializeField] private TMP_InputField _loadMapFilePathText;

        [Header("Create New Map Parameters")]
        [SerializeField] private TMP_InputField _mapNameText;
        [SerializeField] private TMP_InputField _customDirectoryPathText;
        [SerializeField] private string _setFileName = Constants.DEFAULT_FILE_NAME;
        [SerializeField] private Button _createMapButton;

        private void Update()
        {
            _createMapButton.interactable = _toolboxTileSelectionHandler.CurrentlySelectedTilePrefab != null;

            if (_gridSpawner.ActiveMapName != string.Empty)
            {
                _fileNameText.text = _gridSpawner.ActiveMapName;
            }       

            if (!_hasSavedChanges && !_fileNameText.text.EndsWith("*"))
            {
                _fileNameText.text += "*";
            }
            else if (_hasSavedChanges && _fileNameText.text.EndsWith("*"))
            {
                _fileNameText.text = _fileNameText.text.Substring(0, _fileNameText.text.Length - 1);
            }                            
        }

        public void SetUnsavedChanges()
        {
            _hasSavedChanges = false;
        }

        #region Load Map Popup Functions

        public void OpenLoadMapPopup()
        {
            LoadMapPopupManager.ShowPopup();
            _toolboxManager.HideToolbox();
        }

        public void CloseLoadMapPopup()
        {
            LoadMapPopupManager.HidePopup();
        }

        public void SelectMapFromFiles()
        {
#if UNITY_EDITOR
            // Open the file panel in the Unity project's "Assets" directory
            string initialPath = Application.dataPath;
            string path = EditorUtility.OpenFilePanel("Select a Map File", initialPath, "json");

            if (!string.IsNullOrEmpty(path))
            {
                string jsonContents = System.IO.File.ReadAllText(path);
                string FileName = Path.GetFileName(path);
               
                _loadMapFilePathText.text = FileName;

                string formattedFileName = FileName.Replace(".json", "");

                _setFileName = formattedFileName;              
                _gridSpawner.SetActiveMap(formattedFileName);
                _hasSelectedMap = true;               
            }
#endif
        }

        public void LoadMapButton()
        {
            if (_hasSelectedMap)
            {
                Debug.Log("[TACTIKIT/MapEditor] Loaded Map: " + _gridSpawner.ActiveMapName);
                _gridSpawner.LoadActiveMap();
                _loadMapFilePathText.text = string.Empty;
                _hasSelectedMap = false;
                _iterativeSaving = false;
                CloseLoadMapPopup();
                return;
            }
            Debug.LogWarning("[TACTIKIT/MapEditor] Map not selected");
        }

        #endregion // Load Map Popup Functions

        #region Open New Map Popup Functions

        public void OpenNewMapPopup()
        {
            CreateNewMapPopupManager.ShowPopup();
            _toolboxManager.HideToolbox();
        }

        public void CloseNewMapPopup()
        {
            CreateNewMapPopupManager.HidePopup();
        }

        public void SetMapName()
        {
            _setFileName = _mapNameText.text;
        }

        public void ToggleIterativeSaving()
        {
            _iterativeSaving = !_iterativeSaving;
        }

        public void ToggleCustomDirectory()
        {
            _customDirectory = !_customDirectory;
            _customDirectoryPathText.interactable = _customDirectory;
        }

        public void CreateMap()
        {
            _hasSavedChanges = false;
            _gridSpawner.LoadNewMap(_toolboxTileSelectionHandler.CurrentlySelectedTilePrefab);
            
            CloseNewMapPopup();
            _gridSpawner.SetActiveMap(_setFileName);
        }

        #endregion // Open New Map Popup Functions

        #region Save As Popup Functions

        public void OpenSaveAsPopup()
        {
            SaveAsPopupManager.ShowPopup();
            _toolboxManager.HideToolbox();
        }

        public void CloseSaveAsPopup()
        {
            SaveAsPopupManager.HidePopup();
        }

        #endregion // Save As Popup Functions

        public void SaveMap()
        {
            if (_iterativeSaving)
            {
                int nextGridIndex = GetNextGridIndex();
                _hasSavedChanges = _gridSerializer.SaveGrid(nextGridIndex, _setFileName);
                return;
            }
            _hasSavedChanges = _gridSerializer.SaveGrid(_setFileName);
        }

        private int GetNextGridIndex()
        {
            string mapsDirectory = "Assets/Resources/Maps";
            string searchPattern = $"{_setFileName}*.json"; // Pattern to match grid file names

            // Ensure the directory exists
            if (!Directory.Exists(mapsDirectory))
            {
                Directory.CreateDirectory(mapsDirectory);
                return 0;
            }

            string[] files = Directory.GetFiles(mapsDirectory, searchPattern);
            int highestIndex = 0;

            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (int.TryParse(fileName.Substring(_setFileName.Length), out int fileIndex) && fileIndex > highestIndex)
                {
                    highestIndex = fileIndex;
                }
            }

            return highestIndex + 1; // Return the next available index
        }
    }

}