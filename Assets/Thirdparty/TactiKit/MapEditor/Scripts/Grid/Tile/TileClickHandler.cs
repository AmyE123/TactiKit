namespace TactiKit.MapEditor
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// Handles clicking on tiles in the Map Editor.
    /// </summary>
    public class TileClickHandler : MonoBehaviour
    {
        private GameObject _selectedToolboxTile; // The current (new) tile which the user has selected in the toolbox.
        private GameObject _gridParent; // Reference to the Grid GameObject
        private ToolboxTileSelectionHandler _tileSelectionHandler;
        private UIManager _uiManager;
        private Material _originalMaterial;

        private Transform _tileParent;

        [SerializeField] private Material _highlightMaterial;        
        [SerializeField] private Renderer _tileRenderer;

        private void Start()
        {
            _tileRenderer = GetComponent<Renderer>();
            _originalMaterial = _tileRenderer.material;
            _tileParent = transform.parent;

            // Find the TileSelectionHandler in the scene.
            _tileSelectionHandler = FindObjectOfType<ToolboxTileSelectionHandler>();
            _uiManager = FindObjectOfType<UIManager>();

            // Find the Grid GameObject in the scene (make sure it's named "Grid")
            _gridParent = GameObject.Find("Grid");
            if (_gridParent == null)
            {
                Debug.LogError("[TACTIKIT/MapEditor] Grid GameObject not found in the scene!");
            }
        }

        private void Update()
        {
            _selectedToolboxTile = _tileSelectionHandler.CurrentlySelectedTilePrefab;
        }

        private void OnMouseOver()
        {
            _tileRenderer.material = _highlightMaterial;

            if (Input.GetMouseButtonDown(1))
            {
                _tileParent.Rotate(0, 90, 0, Space.Self);
            }
        }

        private void OnMouseExit()
        {
            _tileRenderer.material = _originalMaterial;
        }

        private void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Mouse is over a UI element, do not proceed further
                return;
            }


            if (_selectedToolboxTile != null && _gridParent != null)
            {
                Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);
                GameObject newTile = Instantiate(_selectedToolboxTile, pos, Quaternion.identity);
                newTile.transform.SetParent(_gridParent.transform);

                _uiManager.SetUnsavedChanges();

                // Destroy the old tile
                Destroy(_tileParent.gameObject);
            }
        }
    }
}