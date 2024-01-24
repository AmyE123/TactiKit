namespace CT6GAMAI
{
    using UnityEngine;

    /// <summary>
    /// This class controls the node state
    /// </summary>
    public class NodeState : MonoBehaviour
    {
        public NodeVisualManager VisualStateManager;
        public NodeCursorManager CursorStateManager;

        private void Start()
        {
            VisualStateManager = GetComponent<NodeVisualManager>();
            CursorStateManager = GetComponent<NodeCursorManager>();
        }

        /// <summary>
        /// Change the visual data for a node.
        /// </summary>
        /// <param name="SR">The sprite renderer of the node.</param>
        /// <param name="VisualData">The visual data to change to.</param>
        public void ChangeVisualData(SpriteRenderer SR, NodeVisualData VisualData)
        {
            SR.material = VisualData.Material;
            SR.color = VisualData.Color;
            SR.sprite = VisualData.Sprite;
        }
    }
}