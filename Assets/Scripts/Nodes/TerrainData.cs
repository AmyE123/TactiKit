namespace CT6GAMAI
{
    using UnityEngine;

    /// <summary>
    /// A scriptable object containing all information needed for a terrain.
    /// </summary>
    [CreateAssetMenu(fileName = "TerrainData", menuName = "ScriptableObjects/Terrain/TerrainData", order = 1)]
    public class TerrainData : ScriptableObject
    {
        /// <summary>
        /// The terrain.
        /// </summary>
        public Constants.Terrain TerrainType;

        /// <summary>
        /// What the cost of crossing this terrain is.
        /// </summary>
        public int MovementCost;

        /// <summary>
        /// A defense boost the terrain gives a unit.
        /// </summary>
        public int DefenseBoost;

        /// <summary>
        /// An avoid rate boost the terrain gives a unit.
        /// </summary>
        public int AvoidRateBoost;

        /// <summary>
        /// A heal percentage the terrain gives a unit.
        /// </summary>
        public int HealPercentageBoost;
    }
}