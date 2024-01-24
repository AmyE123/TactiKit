namespace TactiKit.MapEditor
{
    using UnityEditor;
    using UnityEngine;
    using Unity.EditorCoroutines.Editor;

    public class TileImageSaverEditor
    {
        [MenuItem("Tools/TactiKit/MapEditor/Tiles/Update All Tile Images")]
        private static void RefreshAllTileTextures()
        {
            TileImageSaver tileImgSaver = Object.FindObjectOfType<TileImageSaver>();
            if (tileImgSaver == null)
            {
                Debug.LogError("[TACTIKIT/MapEditor] TileImageSaver not found in the scene.");
                return;
            }

            tileImgSaver.CaptureAndSaveAllTiles();
        }

        [MenuItem("Tools/TactiKit/MapEditor/Tiles/Update Tile Image")]
        private static void RefreshTileTexture()
        {
            TileImageSaver tileImgSaver = Object.FindObjectOfType<TileImageSaver>();
            if (tileImgSaver == null)
            {
                Debug.LogError("[TACTIKIT/MapEditor] TileImageSaver not found in the scene.");
                return;
            }

            tileImgSaver.CaptureAndSaveTile(null);
        }
    }
}