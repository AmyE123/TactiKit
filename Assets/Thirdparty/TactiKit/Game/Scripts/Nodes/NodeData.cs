namespace CT6GAMAI
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NodeData", menuName = "ScriptableObjects/Nodes/NodeData", order = 1)]
    public class NodeData : ScriptableObject
    {
        public Vector3 Coordinates;
        public TerrainData TerrainType;
    }
}