namespace CT6GAMAI
{
    using UnityEngine;

    /// <summary>
    /// For the grid cursor to follow the selected node.
    /// This is then referenced on the camera.
    /// </summary>
    public class FollowSelectedNode : MonoBehaviour
    {
        [SerializeField] private GridCursor _gridCursor;

        void Update()
        {
            if (_gridCursor.SelectedNode != null)
            {
                transform.position = _gridCursor.SelectedNode.transform.position;
            }
        }
    }
}
