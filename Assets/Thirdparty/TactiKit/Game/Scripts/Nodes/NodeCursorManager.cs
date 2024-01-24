namespace CT6GAMAI
{
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// A manager for the node cursor, containing controls for the NodeCursorFSM
    /// </summary>
    public class NodeCursorManager : MonoBehaviour
    {
        [SerializeField] private NodeCursorFSM cursorFSM;

        [Header("State Booleans")]
        [SerializeField] private bool _isActiveSelection;
        [SerializeField] private bool _isDefaultSelected;
        [SerializeField] private bool _isPlayerSelected;
        [SerializeField] private bool _isEnemySelected;
        [SerializeField] private bool _isDisabled;

        private GameManager _gameManager;

        [Header("Cursor Visual Data")]
        /// <summary>
        /// The image for the cursor hand
        /// </summary>
        public Image CursorImage;

        /// <summary>
        /// The sprite renderer for the cursor
        /// </summary>
        public SpriteRenderer CursorSR;

        /// <summary>
        /// The gameobject for the sprite renderer.
        /// </summary>
        public GameObject CursorSRGO;

        /// <summary>
        /// The gameObject of the node decal (selection on the tile) for the cursor
        /// </summary>
        public GameObject CursorNodeDecal;

        /// <summary>
        /// The gameObject of the canvas for the cursor
        /// </summary>
        public GameObject CursorCanvas;

        /// <summary>
        /// The different sprites for the cursor
        /// </summary>
        // TODO: Update this into the data scriptable object
        public Sprite[] CursorSprites;

        /// <summary>
        /// Visual data objects for the cursor
        /// </summary>
        public NodeVisualData[] CursorVisualDatas;

        #region Public Getters

        /// <summary>
        /// A bool indicating whether there is an active selection or not.
        /// This returns true if either Default Selected, Player Selected or Enemy Selected states are true.
        /// </summary>
        public bool IsActiveSelection => _isActiveSelection;

        /// <summary>
        /// A bool indicating whether the cursor is in Default state.
        /// </summary>
        public bool IsDefaultSelected => _isDefaultSelected;

        /// <summary>
        /// A bool indicating whether the cursor is over a player
        /// </summary>
        public bool IsPlayerSelected => _isPlayerSelected;

        /// <summary>
        /// A bool indicating whether the cursor is over an enemy
        /// </summary>
        public bool IsEnemySelected => _isEnemySelected;

        /// <summary>
        /// A bool indicating whether the cursor should be disabled for UI events
        /// </summary>
        public bool IsDisabled => _isDisabled;

        #endregion // Public Getters

        #region Hidden In Inspector

        /// <summary>
        /// Reference to NodeState
        /// </summary>
        [HideInInspector] public NodeState State;

        #endregion // Hidden In Inspector

        private void Start()
        {
            cursorFSM = new NodeCursorFSM();
            cursorFSM.Manager = this;

            _gameManager = GameManager.Instance;

            State = GetComponent<NodeState>();

            RefreshCursor();
        }

        private void Awake()
        {
            AnimateCursor();
        }

        // TODO: Add these magic numbers to constants
        private void AnimateCursor()
        {
            DOTween.SetTweensCapacity(1250, 50);
            CursorCanvas.transform.DOLocalMoveY(CURSOR_WSC_DEFAULT_Y_POS + 1, 0.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            CursorSRGO.transform.DOScale(1f, 0.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }

        /// <summary>
        /// Refreshes the cursor to make sure it is up to date with the right state.
        /// </summary>
        private void RefreshCursor()
        {
            CursorCanvas.SetActive(!_isDisabled);
            CursorSRGO.SetActive(!_isDisabled);

            switch (cursorFSM.GetState())
            {
                case NodeCursorState.NoSelection:
                    SetCursorVisuals(CursorSprites[0], false);
                    break;
                case NodeCursorState.DefaultSelected:
                    SetCursorVisuals(CursorSprites[0]);
                    break;
                case NodeCursorState.PlayerSelected:
                    SetCursorVisuals(CursorSprites[0]);
                    break;
                case NodeCursorState.EnemySelected:
                    SetCursorVisualsForEnemySelected();
                    break;
            }
        }

        /// <summary>
        /// This function manages setting the correct state of the cursor when hovered over an enemy unit.
        /// </summary>
        private void SetCursorVisualsForEnemySelected()
        {
            SetCursorVisuals(CursorSprites[isUnitAimingForEnemy() ? 1 : 0], isUnitAimingForEnemy());
        }

        /// <summary>
        /// This returns a boolean for whether the unit is aiming to attack an enemy through pathing.
        /// If the unit is selected, and pathing, and the cursor is over an enemy this will return true.
        /// </summary>
        private bool isUnitAimingForEnemy()
        {
            return _gameManager.GridManager.UnitPressed;
        }

        /// <summary>
        /// Sets the cursor visuals, the image of the cursor, and its active state.
        /// </summary>
        /// <param name="cursorImage">The image that is shown where the cursor is. This can be a hand, an enemy indicator, or something else.</param>
        /// <param name="isActive">Whether the cursor is active or not.</param>
        private void SetCursorVisuals(Sprite cursorImage, bool isActive = true)
        {
            if (!_isDisabled)
            {
                CursorImage.sprite = cursorImage;
                CursorNodeDecal.SetActive(isActive);
                CursorCanvas.SetActive(isActive);
            }
        }

        /// <summary>
        /// Sets the cursor to inactive.
        /// </summary>
        public void SetInactive()
        {
            _isActiveSelection = false;
            _isDefaultSelected = false;
            _isPlayerSelected = false;
            _isEnemySelected = false;
            _isDisabled = false;

            cursorFSM.ChangeState(NodeCursorState.NoSelection);
            RefreshCursor();
        }

        /// <summary>
        /// Sets the cursor to default selected.
        /// </summary>
        public void SetDefaultSelected()
        {
            _isActiveSelection = true;
            _isDefaultSelected = true;
            _isPlayerSelected = false;
            _isEnemySelected = false;
            _isDisabled = false;

            cursorFSM.ChangeState(NodeCursorState.DefaultSelected);
            RefreshCursor();
        }

        /// <summary>
        /// Sets the cursor to player selected (when the cursor is over a player)
        /// </summary>
        public void SetPlayerSelected()
        {
            _isActiveSelection = true;
            _isDefaultSelected = false;
            _isPlayerSelected = true;
            _isEnemySelected = false;
            _isDisabled = false;

            cursorFSM.ChangeState(NodeCursorState.PlayerSelected);
            RefreshCursor();
        }

        /// <summary>
        /// Sets the cursor to enemy selected (when the cursor is over an enemy)
        /// </summary>
        public void SetEnemySelected()
        {
            _isActiveSelection = true;
            _isDefaultSelected = false;
            _isPlayerSelected = false;
            _isEnemySelected = true;
            _isDisabled = false;

            cursorFSM.ChangeState(NodeCursorState.EnemySelected);
            RefreshCursor();
        }

        /// <summary>
        /// Sets the cursor to diabled for UI events
        /// </summary>
        public void SetDisabled()
        {
            _isDisabled = true;
            RefreshCursor();
        }

        /// <summary>
        /// Sets the cursor to enabled for UI events
        /// </summary>
        public void SetEnabled()
        {
            _isDisabled = false;
            RefreshCursor();
        }
    }
}
