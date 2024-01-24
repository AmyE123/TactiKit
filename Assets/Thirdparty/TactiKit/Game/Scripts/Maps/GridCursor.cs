namespace CT6GAMAI
{
    using System.Collections.Generic;
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// Manages the selection and interaction of nodes within the grid.
    /// This includes handling node selection, grid navigation, unit selection, and pathing.
    /// </summary>
    public class GridCursor : MonoBehaviour
    {
        [Header("Cursor Configuration")]
        [SerializeField] private bool _enablePlayerInput;

        [Header("Audio")]
        [SerializeField] private AudioSource _cursorAudioSource;
        [SerializeField] private CursorAudioClips _cursorAudioClips;

        [Header("Node Selection")]
        /// <summary>
        /// The currently selected node.
        /// </summary>
        public NodeManager SelectedNode;

        /// <summary>
        /// The state of the currently selected node.
        /// </summary>
        public NodeState SelectedNodeState;

        [Header("Unit Interaction")]
        /// <summary>
        /// Indicates whether a unit is currently pressed (selected).
        /// </summary>
        public bool UnitPressed = false;

        #region Public Getters

        /// <summary>
        /// Indicates whether pathing mode is active.
        /// </summary>
        public bool Pathing => _pathing;

        /// <summary>
        /// Indicated whether player input is enabled or not.
        /// </summary>
        public bool EnablePlayerInput => _enablePlayerInput;

        #endregion // Public Getters

        private GameManager _gameManager;
        private GridManager _gridManager;
        private AudioManager _audioManager;
        private UnitManager _lastSelectedUnit;
        private bool _pathing = false;
        private int _playerSelectedIDX = 0;

        private void Start()
        {
            InitializeValues();
        }

        private void Update()
        {
            _pathing = UnitPressed;

            if (_enablePlayerInput)
            {
                UpdateSelectedNode();
                HandleNodeUnitInteraction();
                HandleGridNavigation();
                HandleUnitSelection();
                HandleUnitPathing();
            }
            else
            {
                ResetHighlightedNodes();
                HandleNodeUnitInteraction();
            }
        }

        private void InitializeValues()
        {
            _gameManager = GameManager.Instance;
            _gridManager = _gameManager.GridManager;
            _audioManager = _gameManager.AudioManager;
        }

        private void UpdateUnitReferences()
        {
            var unitManager = _gameManager.UnitsManager;
            _lastSelectedUnit = unitManager.ActiveUnit;
        }

        private void UpdateTerrainTypeUI()
        {
            _gameManager.UIManager.TileInfoManager.SetTerrainType(SelectedNode.NodeData);
        }

        private void UpdateSelectedNode()
        {
            if (SelectedNodeState == null)
            {
                SelectedNodeState = SelectedNode.NodeState;
                SelectedNodeState.CursorStateManager.SetDefaultSelected();
            }

            if (SelectedNode == null || !SelectedNodeState.CursorStateManager.IsActiveSelection)
            {
                UpdateActiveNodeSelection();
            }

            if (Input.GetKeyDown(KeyCode.E) && !_pathing && !UnitPressed)
            {
                _playerSelectedIDX++;

                if (_playerSelectedIDX >= _gameManager.UnitsManager.UnplayedPlayerUnits.Count)
                {
                    _playerSelectedIDX = 0;
                }

                MoveCursorTo(_gameManager.UnitsManager.UnplayedPlayerUnits[_playerSelectedIDX].StoodNode.Node);
            }
        }

        private void UpdateActiveNodeSelection()
        {
            var nodes = _gridManager.AllNodes;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].NodeState.CursorStateManager.IsActiveSelection)
                {
                    SelectedNode = nodes[i];
                    SelectedNodeState = SelectedNode.NodeState;
                }
            }
        }

        private void HandleNodeUnitInteraction()
        {
            _gameManager.UnitsManager.SetCursorUnit(SelectedNode.StoodUnit);

            if (SelectedNode.StoodUnit != null)
            {
                ManageUnitSelection();

                if (_pathing)
                {
                    HandlePathingInteraction();
                }
            }
            else
            {
                ResetUnitAndUI();
            }
        }

        private void ManageUnitSelection()
        {
            _gameManager.UIManager.UnitInfoManager.SetUnitType(SelectedNode.StoodUnit.UnitData);

            if (!_pathing || _gameManager.UnitsManager.ActiveUnit == null)
            {
                _gameManager.UnitsManager.SetActiveUnit(SelectedNode.StoodUnit);
                HandlePlayerUnitSelection();
            }
        }

        private void HandlePlayerUnitSelection()
        {
            if (SelectedNode.StoodUnit.UnitData.UnitTeam != Team.Enemy)
            {
                _gameManager.UnitsManager.SetLastSelectedPlayerUnit(_gameManager.UnitsManager.ActiveUnit);
            }

            ResetHighlightedNodes();
            SelectedNode.HighlightRangeArea(SelectedNode.StoodUnit, SelectedNodeState.VisualStateManager.IsPressed);
        }

        private void HandlePathingInteraction()
        {
            if (_gameManager.UnitsManager.CursorUnit.UnitData.UnitTeam == Team.Enemy)
            {
                SelectedNode.NodeState.CursorStateManager.SetEnemySelected();
            }
            else if (_gameManager.UnitsManager.CursorUnit.UnitData.UnitTeam == Team.Player)
            {
                SelectedNode.NodeState.CursorStateManager.SetPlayerSelected();
            }
            else
            {
                SelectedNode.NodeState.CursorStateManager.SetDefaultSelected();
            }
        }

        private void ResetUnitAndUI()
        {
            _gameManager.UIManager.UnitInfoManager.SetUnitInfoUIInactive();

            if (!UnitPressed)
            {
                _gameManager.UnitsManager.SetActiveUnit(null);
                ResetHighlightedNodes();
            }
        }

        private void HandleGridNavigation()
        {
            UpdateUnitReferences();
            UpdateTerrainTypeUI();

            if (CanCursorMove())
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    MoveCursor(Direction.North);
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    MoveCursor(Direction.West);
                }

                if (Input.GetKeyDown(KeyCode.S))
                {
                    MoveCursor(Direction.South);
                }

                if (Input.GetKeyDown(KeyCode.D))
                {
                    MoveCursor(Direction.East);
                }
            }
        }

        private bool CanCursorMove()
        {
            bool IsAnyUnitMoving = _gameManager.UnitsManager.IsAnyUnitMoving();
            bool IsActionItemsActive = _gameManager.UIManager.ActionItemsManager.IsActionItemsActive;
            bool IsUnitConfirmingMove = _gameManager.GridManager.CurrentState == CurrentState.ConfirmingMove;
            bool IsBattleActive = _gameManager.CameraManager.CameraState == CameraStates.Battle;

            return !IsAnyUnitMoving && !IsActionItemsActive && !IsUnitConfirmingMove && !IsBattleActive;
        }

        private void MoveCursor(Direction direction)
        {
            _audioManager.PlayCursorSound(UnitPressed);

            NodeState adjacentNodeState = GetAdjacentNodeState(direction);
            if (adjacentNodeState != null)
            {
                ResetCursors();
                adjacentNodeState.CursorStateManager.SetDefaultSelected();
            }
        }

        private NodeState GetAdjacentNodeState(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return SelectedNode.NorthNode.NodeState;
                case Direction.South:
                    return SelectedNode.SouthNode.NodeState;
                case Direction.West:
                    return SelectedNode.WestNode.NodeState;
                case Direction.East:
                    return SelectedNode.EastNode.NodeState;
                default:
                    Debug.Assert(false, "Invalid direction passed to GetAdjacentNodeState.");
                    return null;
            }
        }

        private void HandleUnitSelection()
        {
            if (CheckGameStateConditions())
            {
                ProcessConfirmPress();
            }
        }

        private bool CheckGameStateConditions()
        {
            bool isConfirmingMove = _gameManager.GridManager.CurrentState == CurrentState.ConfirmingMove;
            bool isCameraInBattle = _gameManager.CameraManager.CameraState == CameraStates.Battle;

            return !isConfirmingMove && !isCameraInBattle;
        }

        private void ProcessConfirmPress()
        {
            bool isConfirmPressed = Input.GetKeyDown(KeyCode.Space);
            bool isActionSelected = _gameManager.GridManager.CurrentState == CurrentState.ActionSelected;

            if (isConfirmPressed && !isActionSelected)
            {
                bool isCursorUnitValid = CheckCursorUnitValidity();
                if (!isCursorUnitValid)
                {
                    ToggleUnitSelection();
                }
            }
        }

        private bool CheckCursorUnitValidity()
        {
            bool isPathing = _pathing;
            bool isCursorActive = _gameManager.UnitsManager.CursorUnit != null;
            bool isCursorOnActiveUnit = _gameManager.UnitsManager.CursorUnit == _gameManager.UnitsManager.ActiveUnit;

            return isPathing && isCursorActive && !isCursorOnActiveUnit;
        }

        private void ToggleUnitSelection()
        {
            bool hasUnitActedThisTurn = _gameManager.UnitsManager.ActiveUnit.HasActedThisTurn;

            if (SelectedNode.StoodUnit != null && !hasUnitActedThisTurn && SelectedNode.StoodUnit.UnitData.UnitTeam != Team.Enemy)
            {
                UnitPressed = !UnitPressed;

                _audioManager.PlayToggleUnitSound(UnitPressed);

                UpdateNodeVisualState();
            }
        }

        private void UpdateNodeVisualState()
        {
            if (UnitPressed)
            {
                _gameManager.UnitsManager.ActiveUnit.IsSelected = true;
                SelectedNodeState.VisualStateManager.SetPressed(NodeVisualColorState.Blue);

                foreach (Node n in _lastSelectedUnit.MovementRange.ReachableNodes)
                {
                    n.NodeManager.NodeState.VisualStateManager.SetPressed(NodeVisualColorState.Blue);
                }
            }

            if (!UnitPressed)
            {
                _gameManager.UnitsManager.ActiveUnit.IsSelected = false;
                SelectedNodeState.VisualStateManager.SetHovered(NodeVisualColorState.Blue);

                foreach (Node n in _lastSelectedUnit.MovementRange.ReachableNodes)
                {
                    n.NodeManager.NodeState.VisualStateManager.SetHovered(NodeVisualColorState.Blue);
                }
            }
        }

        private void HandleUnitPathing()
        {
            if (!UnitPressed)
            {
                return;
            }

            _gridManager.HandleUnitPathing();
        }

        private void ResetCursors()
        {
            var nodes = _gridManager.AllNodes;
            foreach (NodeManager n in nodes)
            {
                n.NodeState.CursorStateManager.SetInactive();
            }
        }

        /// <summary>
        /// Moves the cursor to a specified place. Used for enemy AI.
        /// </summary>
        /// <param name="node">The node you want to move the cursor to.</param>
        public void MoveCursorTo(Node node)
        {
            ResetCursors();

            node.NodeManager.NodeState.CursorStateManager.SetDefaultSelected();
            SelectedNode = node.NodeManager;
        }

        /// <summary>
        /// Sets the player input. Used for the turn based system.
        /// </summary>
        /// <param name="value">the value you want to set the player input to</param>
        public void SetPlayerInput(bool value)
        {
            _enablePlayerInput = value;
        }

        /// <summary>
        /// Resets the visual state of all highlighted nodes to default.
        /// </summary>
        public void ResetHighlightedNodes()
        {
            var nodes = _gridManager.AllNodes;

            if (_lastSelectedUnit != null)
            {
                _lastSelectedUnit.MovementRange.ResetNodes();
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].NodeState.VisualStateManager.IsActive)
                {
                    nodes[i].NodeState.VisualStateManager.SetDefault();
                }

                SelectedNodeState.VisualStateManager.SetDefault();
            }
        }
    }
}