namespace TactiKit.MapEditor
{
    using UnityEngine;

    /// <summary>
    /// Stores all relevant information about the currently selected tile in the toolbox.
    /// </summary>
    public class ToolboxTileSelectionHandler : MonoBehaviour
    {
        private GameObject _currentlySelectedTilePrefab;

        /// <summary>
        /// A public reference to the currently selected tile.
        /// </summary>
        public GameObject CurrentlySelectedTilePrefab => _currentlySelectedTilePrefab;

        /// <summary>
        /// Sets the active selected tile.
        /// </summary>
        public void SetActiveTile(GameObject activePrefab)
        {
            _currentlySelectedTilePrefab = activePrefab;
        }
    }
}