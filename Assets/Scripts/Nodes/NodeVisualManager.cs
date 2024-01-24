namespace CT6GAMAI
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// A manager for the node visuals (on the ground), containing controls for the NodeVisualFSM
    /// </summary>
    public class NodeVisualManager : MonoBehaviour
    {
        [SerializeField] private NodeVisualFSM visualFSM;

        [Header("State Booleans")]
        [SerializeField] private bool _isActive;
        [SerializeField] private bool _isDefault;
        [SerializeField] private bool _isHovered;
        [SerializeField] private bool _isPressed;
        [SerializeField] private bool _isPath;
        [SerializeField] private bool _isDisabled;

        [Header("Node Visual Data")]
        /// <summary>
        /// The sprite renderer for the node visual
        /// </summary>
        public SpriteRenderer VisualSR;

        /// <summary>
        /// Visual data objects for the node visual
        /// </summary>
        public NodeVisualData[] VisualDatas;

        #region Public Getters

        /// <summary>
        /// A bool indicating whether the node visual is active or not.
        /// This returns true if any of the other bools besides default (Hovered, Pressed, Path) is true.
        /// </summary>
        public bool IsActive => _isActive;

        /// <summary>
        /// A bool indicating whether the visual is in default state. Default = No Visuals.
        /// </summary>
        public bool IsDefault => _isDefault;

        /// <summary>
        /// A bool indicating whether the visual is in a hovered state.
        /// Hovered is when the selection is over a unit and it shows their movement range.
        /// </summary>
        public bool IsHovered => _isHovered;

        /// <summary>
        /// A bool indicating whether the visual is in a pressed state.
        /// Pressed is when a unit has been pressed and it highlights the movement range in bold.
        /// </summary>
        public bool IsPressed => _isPressed;

        /// <summary>
        /// A bool indicating whether the visual is in a path state.
        /// Path is when the unit's path has been selected, and this is a highlight of it.
        /// </summary>
        public bool IsPath => _isPath;

        /// <summary>
        /// A bool indicating whether the visual is in a disabled state.
        /// </summary>
        public bool IsDisabled => _isDisabled;

        #endregion // Public Getters

        #region Hidden In Inspector

        [HideInInspector]
        public NodeState State;

        #endregion // Hidden In Inspector

        private void Start()
        {
            visualFSM = new NodeVisualFSM();
            visualFSM.Manager = this;

            State = GetComponent<NodeState>();
        }

        private void Update()
        {
            VisualSR.gameObject.SetActive(!_isDisabled);
        }

        public void SetDefault()
        {
            _isActive = false;
            _isDefault = true;
            _isHovered = false;
            _isPressed = false;
            _isPath = false;

            visualFSM.ChangeState(NodeVisualState.Default);
        }

        public void SetHovered(NodeVisualColorState color)
        {
            _isActive = true;
            _isDefault = false;
            _isHovered = true;
            _isPressed = false;
            _isPath = false;

            switch (color)
            {
                case NodeVisualColorState.Blue:
                    visualFSM.ChangeState(NodeVisualState.HoveredBlue);
                    break;
                case NodeVisualColorState.Red:
                    visualFSM.ChangeState(NodeVisualState.HoveredRed);
                    break;
                case NodeVisualColorState.Green:
                    visualFSM.ChangeState(NodeVisualState.HoveredGreen);
                    break;
            }
        }

        public void SetPressed(NodeVisualColorState color)
        {
            _isActive = true;
            _isDefault = false;
            _isHovered = false;
            _isPressed = true;
            _isPath = false;

            switch (color)
            {
                case NodeVisualColorState.Blue:
                    visualFSM.ChangeState(NodeVisualState.SelectedBlue);
                    break;
                case NodeVisualColorState.Red:
                    visualFSM.ChangeState(NodeVisualState.SelectedRed);
                    break;
                case NodeVisualColorState.Green:
                    visualFSM.ChangeState(NodeVisualState.SelectedGreen);
                    break;
            }
        }

        public void SetPath()
        {
            _isActive = true;
            _isDefault = false;
            _isHovered = false;
            _isPressed = false;
            _isPath = true;

            visualFSM.ChangeState(NodeVisualState.PointOfInterest);
        }

        /// <summary>
        /// Sets the cursor to diabled for UI events
        /// </summary>
        public void SetDisabled()
        {
            _isDisabled = true;
        }

        /// <summary>
        /// Sets the cursor to enabled for UI events
        /// </summary>
        public void SetEnabled()
        {
            _isDisabled = false;
        }
    }
}
