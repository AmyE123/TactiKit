namespace TactiKit.MapEditor
{
    using System;
    using TMPro;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.UIElements;
    using static TactiKit.MapEditor.Constants;
    public class UI_TileButtonsLoader : MonoBehaviour
    {
        [SerializeField] private ToolboxTileSelectionHandler _tileSelectionHandler;
        [SerializeField] private ToolboxManager _toolboxManager;

        [SerializeField] private GameObject _tileButtonPrefab;
        [SerializeField] private Transform _tileToolboxContentArea;

        [SerializeField] private GameObject _tabButtonPrefab;
        [SerializeField] private Transform _tabToolboxContentArea;

        [SerializeField] private GameObject _smallTileButtonPrefab;
        [SerializeField] private Transform _newMapTerrainContentArea;

        private TileTypes _activeTileType;

        private void Start()
        {
            LoadTileTabs();
            LoadToolboxTileButtons();
            LoadNewMapTerrainTileButtons();
        }

        private void LoadToolboxTileButtons()
        {
            LoadTileButtons(_tileToolboxContentArea, _tileButtonPrefab);
        }

        private void LoadNewMapTerrainTileButtons()
        {
            LoadTileButtons(_newMapTerrainContentArea, _smallTileButtonPrefab, true, false);
        }

        private void LoadTileTabs()
        {
            foreach (string name in Enum.GetNames(typeof(TileTypes)))
            {
                GameObject button = Instantiate(_tabButtonPrefab, _tabToolboxContentArea);
                var textUI = button.GetComponentInChildren<TMP_Text>(true);
                textUI.text = name;

                button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => SelectTab(name));
            }
        }
        private void LoadTileButtons(Transform contentParent, GameObject buttonPrefab, bool ignoreTileType = false, bool isToolboxTile = true)
        {
            ClearContent(contentParent);

            GameObject[] tilePrefabs = Resources.LoadAll<GameObject>("Tiles");
            foreach (GameObject tilePrefab in tilePrefabs)
            {
                if (ignoreTileType || tilePrefab.GetComponent<TileData>().TileType == _activeTileType)
                {
                    GameObject button = CreateTileButton(contentParent, buttonPrefab, tilePrefab);
                    button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => SelectTilePrefab(tilePrefab, isToolboxTile));
                }
            }
        }

        private GameObject CreateTileButton(Transform contentParent, GameObject buttonPrefab, GameObject tilePrefab)
        {
            GameObject button = Instantiate(buttonPrefab, contentParent);
            TMP_Text textUI = button.GetComponentInChildren<TMP_Text>(true);
            textUI.text = tilePrefab.name;

            foreach (Transform child in button.transform)
            {
                if (child.name != "Divider")
                {
                    var image = child.GetComponent<UnityEngine.UI.Image>();
                    image.sprite = FindTileImage(tilePrefab.name);
                    if (image != null)
                    {
                        break;
                    }
                }
            }

            return button;
        }

        private void ClearContent(Transform contentParent)
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
        }

        private Sprite FindTileImage(string tileName)
        {
            string path = $"{TILE_TEXTURES_DIRECTORY}{tileName}{ASSET_FILE_TYPE}";
            Texture2D texture = null;

#if UNITY_EDITOR

            texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

#endif // UNITY_EDITOR

            if (texture == null)
            {
                Debug.LogError($"[TACTIKIT/MapEditor] Tile Texture not found for: {tileName}. \n You may have created a new tile and not added or updated your tile textures (Tools/TactiKit/MapEditor/Tools/Update Tile Image) for {tileName}{ASSET_FILE_TYPE}");
                return null;
            }

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }

        private void SelectTab(string name)
        {
            _activeTileType = (TileTypes)System.Enum.Parse(typeof(TileTypes), name);
            LoadTileButtons(_tileToolboxContentArea, _tileButtonPrefab);
        }

        private void SelectTilePrefab(GameObject prefab, bool shouldToggleToolbox = true)
        {
            _tileSelectionHandler.SetActiveTile(prefab);

            if (shouldToggleToolbox)
            {
                _toolboxManager.HideToolbox();
            }

        }
    }
}