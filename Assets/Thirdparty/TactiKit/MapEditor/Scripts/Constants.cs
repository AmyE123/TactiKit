namespace TactiKit.MapEditor
{
    /// <summary>
    /// Contains constants and enums used throughout the map editor.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The different tile types.
        /// </summary>
        public enum TileTypes { Plain, River, Unwalkable, Fort, Forest, Bridge }

        /// <summary>
        /// The default size of the grid for a map.
        /// </summary>
        public const int DEFAULT_GRID_SIZE = 20;

        /// <summary>
        /// The default size of a tile on the grid.
        /// </summary>
        public const float DEFAULT_TILE_SIZE = 1;

        /// <summary>
        /// The default file type for a map.
        /// </summary>
        public const string DEFAULT_FILE_TYPE = ".json";

        /// <summary>
        /// The file type string for the assets.
        /// </summary>
        public const string ASSET_FILE_TYPE = ".asset";

        /// <summary>
        /// The default file name for a map.
        /// </summary>
        public const string DEFAULT_FILE_NAME = "Map";

        /// <summary>
        /// The default saving directory.
        /// </summary>
        public const string DEFAULT_DIRECTORY = "Assets/Resources/Maps/";

        /// <summary>
        /// The name of the resources folder containing all the Tiles.
        /// </summary>
        public const string TILE_RESOURCES_FOLDER = "Tiles";

        /// <summary>
        /// The directory containing all of the tile rendered textures.
        /// </summary>
        public const string TILE_TEXTURES_DIRECTORY = "Assets/ThirdParty/TactiKit/MapEditor/Textures/TileThumbnailRenders/";
    }
}