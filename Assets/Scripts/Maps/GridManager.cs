namespace CT6GAMAI
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// Manages the grid for the map.
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private GridCursor _gridCursor;
        [SerializeField] private List<NodeManager> _allNodes;
        [SerializeField] private List<NodeManager> _occupiedNodes;
        [SerializeField] private List<Node> _movementPath;
        [SerializeField] private bool _canSwitchToBattle = false;

        private UnitManager _activeUnit;
        private GameManager _gameManager;
        private bool _cursorWithinRange;
        private bool _gridInitialized = false;

        /// <summary>
        /// The current state of the grid cursor in regards to unit actions.
        /// </summary>
        public CurrentState CurrentState;

        /// <summary>
        /// A reference to the grid cursor class.
        /// </summary>
        public GridCursor GridCursor => _gridCursor;

        /// <summary>
        /// Gets the list of all nodes in the grid.
        /// </summary>
        public List<NodeManager> AllNodes => _allNodes;

        /// <summary>
        /// Gets the movement path of the currently selected unit.
        /// </summary>
        public List<Node> MovementPath => _movementPath;

        /// <summary>
        /// Whether a unit is currently pressed or not
        /// </summary>
        public bool UnitPressed => _gridCursor.UnitPressed;

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        private void Update()
        {
            UpdateUnitReferences();

            if (!_gridInitialized)
            {
                InitializeGrid();
            }

            if (CurrentState == CurrentState.ConfirmingMove && _gameManager.CameraManager.CameraState != CameraStates.Battle)
            {
                var unit = _gameManager.UnitsManager.ActiveUnit;

                if (unit != null && unit.IsAwaitingMoveConfirmation)
                {
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        if (_gameManager.UIManager.BattleForecastManager.AreBattleForecastsToggled)
                        {
                            _gameManager.UIManager.BattleForecastManager.CancelBattleForecast();
                        }

                        unit.CancelMove();
                        unit.IsAwaitingMoveConfirmation = false;
                        _canSwitchToBattle = false;
                    }

                    if (_canSwitchToBattle && Input.GetKeyDown(KeyCode.Space))
                    {
                        _gameManager.BattleManager.SwitchToBattle(Team.Player);
                        _canSwitchToBattle = false;
                    }
                }
            }

            if (CurrentState == CurrentState.ActionSelected)
            {
                StartCoroutine(IdleDelay());
            }
        }

        IEnumerator IdleDelay()
        {
            yield return new WaitForSeconds(IDLE_DELAY);
            CurrentState = CurrentState.Idle;
        }

        private IEnumerator BattleTransition()
        {
            yield return new WaitForSeconds(0.1f);
            _canSwitchToBattle = true;
        }

        private void UpdateUnitReferences()
        {
            _activeUnit = _gameManager.UnitsManager.ActiveUnit;
        }

        private void InitializeGrid()
        {
            if (CheckNodesInitialized())
            {
                CheckForOccupiedNodes();
                _gridInitialized = true;
            }

            if (_allNodes.Count == 0)
            {
                FindAllNodes();
            }
        }

        private bool CheckNodesInitialized()
        {
            foreach (NodeManager n in _allNodes)
            {
                if (!n.NodeInitialized)
                {
                    return false;
                }
            }

            return true;
        }

        private void FindAllNodes()
        {
            _allNodes = FindObjectsOfType<NodeManager>().ToList();
        }

        private void CheckForOccupiedNodes()
        {
            foreach (NodeManager n in _allNodes)
            {
                if (n.StoodUnit != null)
                {
                    if (!_occupiedNodes.Contains(n))
                    {
                        _occupiedNodes.Add(n);
                    }
                }
            }
        }

        private void HighlightPath()
        {
            foreach (Node n in _movementPath)
            {
                if (_activeUnit.MovementRange.ReachableNodes.Contains(n))
                {
                    n.NodeManager.NodeState.VisualStateManager.SetPath();
                }
            }
        }

        private void ProcessMovementPath()
        {
            if (_gameManager.UnitsManager.ActiveUnit.StoodNode != null)
            {
                Node startNode = _gameManager.UnitsManager.ActiveUnit.StoodNode.Node;
                Node targetNode = _gridCursor.SelectedNode.Node;

                if (!_cursorWithinRange)
                {
                    return;
                }

                _movementPath = _activeUnit.MovementRange.ReconstructPath(startNode, targetNode);
                HandleMovementInput(targetNode);
            }
        }

        private void HandleMovementInput(Node targetNode)
        {
            if (CurrentState != CurrentState.ConfirmingMove &&
                CurrentState != CurrentState.Moving &&
                _gameManager.CameraManager.CameraState != CameraStates.Battle &&
                Input.GetKeyDown(KeyCode.Space))
            {
                ProcessUnitMovement(targetNode);
            }
        }

        private void ProcessUnitMovement(Node targetNode)
        {
            var validPath = _movementPath.Count > 1 && CanMoveToNode(targetNode);
            _gameManager.AudioManager.PlaySelectPathSound(validPath);

            if (validPath)
            {
                PerformValidPathMovement();
            }
            else
            {
                HandleInvalidPath(targetNode);
            }
        }

        private void PerformValidPathMovement()
        {
            _gameManager.UIManager.ActionItemsManager.ShowActionItems();
            var unit = _gameManager.UnitsManager.ActiveUnit;
            unit.ClearStoodUnit();
            StartCoroutine(unit.MoveToEndPoint());
        }

        private void HandleInvalidPath(Node targetNode)
        {
            if (isNodeOccupiedByEnemy(targetNode) && CurrentState != CurrentState.ConfirmingMove)
            {
                CurrentState = CurrentState.ConfirmingMove;
                _activeUnit.IsAwaitingMoveConfirmation = true;
                var unit = _gameManager.UnitsManager.ActiveUnit;
                if (_gameManager.GridManager.MovementPath.Count > 2)
                {
                    unit.ClearStoodUnit();
                    StartCoroutine(unit.MoveToEndPoint(1));
                }
                else
                {
                    _activeUnit.IsAwaitingMoveConfirmation = true;
                    _gameManager.GridManager.CurrentState = CurrentState.ConfirmingMove;
                }

                _gameManager.UIManager.BattleForecastManager.SpawnBattleForecast(_activeUnit, targetNode.NodeManager.StoodUnit);
                StartCoroutine(BattleTransition());
            }
        }

        /// <summary>
        /// Checks if a movement to a node is valid based on current game rules.
        /// </summary>
        /// <returns>
        /// False if the node is occupied by a unit other than the last selected unit. Otherwise true.
        /// </returns>
        public bool CanMoveToNode(Node node)
        {
            // Checks if the node is occupied by a unit other than the last selected one.
            var currentUnitOnNode = GetOccupyingUnitFromNode(node);
            var nodeOccupiedByOtherUnit = currentUnitOnNode != null && currentUnitOnNode != _gameManager.UnitsManager.ActiveUnit;

            bool canMoveToNode = !nodeOccupiedByOtherUnit;

            return canMoveToNode;
        }

        /// <summary>
        /// Gets the occupying unit from a node.
        /// </summary>
        public UnitManager GetOccupyingUnitFromNode(Node node)
        {
            return node.NodeManager.StoodUnit;
        }

        /// <summary>
        /// Checks if a node is occupied by an enemy.
        /// </summary>
        public bool isNodeOccupiedByEnemy(Node node)
        {
            var currentUnitOnNode = GetOccupyingUnitFromNode(node);
            return currentUnitOnNode != null && currentUnitOnNode.UnitData.UnitTeam == Team.Enemy;
        }

        /// <summary>
        /// Handles the pathing logic when a unit is selected.
        /// </summary>
        public void HandleUnitPathing()
        {
            UpdateCursorRange();
            ProcessPathing();
        }

        /// <summary>
        /// Updates whether the cursor is within range of the reachable nodes.
        /// </summary>
        public void UpdateCursorRange()
        {
            foreach (Node n in _activeUnit.MovementRange.ReachableNodes)
            {
                _cursorWithinRange = _activeUnit.MovementRange.ReachableNodes.Contains(_gridCursor.SelectedNode.Node);
                n.NodeManager.NodeState.VisualStateManager.SetPressed(NodeVisualColorState.Blue);
            }
        }

        /// <summary>
        /// Processes pathing for the selected unit, including movement and path highlighting.
        /// </summary>
        public void ProcessPathing()
        {
            ProcessMovementPath();
            HighlightPath();
        }
    }
}