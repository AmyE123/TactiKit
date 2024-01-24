namespace CT6GAMAI
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Node : MonoBehaviour
    {
        #region Private Variables

        // TODO: Figure out a better way to deal with this
        [SerializeField] private NodeManager _manager;

        #endregion // Private Variables

        // TODO: Move public variables to private
        #region Public Variables

        // The cost to enter this node
        // TODO: Make this update with the terrain data
        public int Cost = 1;

        /// <summary>
        /// The distance used for Dijkstra's algorithm
        /// </summary>
        public int Distance { get; set; }

        /// <summary>
        /// Whether the node has been visited
        /// </summary>
        public bool Visited = false;

        /// <summary>
        /// Whether the node is walkable
        /// </summary>
        public bool Walkable;

        /// <summary>
        /// A list of adjacent nodes (neighbors)
        /// </summary>
        public List<Node> Neighbors;

        /// <summary>
        /// For movement algorithm, store the predecessor
        /// </summary>
        public Node Predecessor { get; set; }

        /// <summary>
        /// A unit transform on the node
        /// </summary>
        public Transform UnitTransform;

        #endregion // Public Variables

        #region Public Getters

        /// <summary>
        /// A getter for the NodeManager for this node.
        /// </summary>
        public NodeManager NodeManager => _manager;

        #endregion // Public Getters

        public void AddNeighbor(Node neighbor)
        {
            Neighbors.Add(neighbor);
        }
    }
}
