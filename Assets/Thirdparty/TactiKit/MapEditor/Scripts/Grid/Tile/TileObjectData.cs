namespace TactiKit.MapEditor
{
    using UnityEngine;

    /// <summary>
    /// This contains information about a tile regarding the game object.
    /// Used for save/loading the assets.
    /// </summary>
    [System.Serializable]
    public class TileObjectData
    {
        public float[] Position;
        public string PrefabName;

        public TileObjectData(GameObject gameObject)
        {
            Position = new float[] { gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z };
            PrefabName = GetCleanPrefabName(gameObject.name);
        }

        private string GetCleanPrefabName(string prefabName)
        {
            return prefabName.Replace("(Clone)", ""); // Remove "(Clone)" from the name
        }
    }
}