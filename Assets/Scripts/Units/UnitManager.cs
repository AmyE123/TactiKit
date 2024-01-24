namespace CT6GAMAI
{
    using UnityEngine;
    using DG.Tweening;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using static CT6GAMAI.Constants;
    using System.Net;

    /// <summary>
    /// Manager for the singular unit.
    /// </summary>
    public class UnitManager : MonoBehaviour
    {
        [Header("Unit Components")]
        [SerializeField] private MovementRange _movementRange;
        [SerializeField] private UnitAnimationManager _unitAnimationManager;
        [SerializeField] private UnitStatsManager _unitStatsManager;
        [SerializeField] private GameObject _battleUnit;

        [Header("Unit State")]
        [SerializeField] private UnitData _unitData;
        [SerializeField] private NodeManager _stoodNode;
        [SerializeField] private NodeManager _updatedStoodNode;
        [SerializeField] private Animator _animator;
        [SerializeField] private bool _isAwaitingMoveConfirmation;

        [Header("Unit Visuals")]
        [SerializeField] private Material _inactiveMaterial;
        [SerializeField] private Material _activeMaterial;
        [SerializeField] private GameObject _modelBaseObject;
        [SerializeField] private List<Renderer> _allRenderers;

        private GameManager _gameManager;
        private GridManager _gridManager;
        private TurnManager _turnManager;
        private RaycastHit _stoodNodeRayHit;
        private GridCursor _gridCursor;
        private bool _isMoving = false;
        private List<SkinnedMeshRenderer> _allSMRRenderers;
        private List<MeshRenderer> _allMRRenderers;
        private bool _isSelected = false;
        private bool _unitDead = false;
        private bool _hasActedThisTurn = false;
        private bool _canActThisTurn = false;
        private bool _isInBattle = false;

        /// <summary>
        /// Whether this unit has been selected by the cursor or not.
        /// </summary>
        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; } }

        /// <summary>
        /// Whether this unit is awaiting a move confirmation.
        /// </summary>
        public bool IsAwaitingMoveConfirmation { get { return _isAwaitingMoveConfirmation; } set { _isAwaitingMoveConfirmation = value; } }

        /// <summary>
        /// Whether this unit is currently within a battle animation or not.
        /// </summary>
        public bool IsInBattle { get { return _isInBattle; } set { _isInBattle = value; } }

        /// <summary>
        /// Gets a bool indicating whether the unit is currently moving or not.
        /// </summary>
        public bool IsMoving => _isMoving;

        /// <summary>
        /// The node that the unit is stood on.
        /// </summary>
        public NodeManager StoodNode => _stoodNode;

        /// <summary>
        /// The node that the unit is currently over. Updated all the time.
        /// </summary>
        public NodeManager UpdatedStoodNode => _updatedStoodNode;

        /// <summary>
        /// The units data.
        /// </summary>
        public UnitData UnitData => _unitData;

        /// <summary>
        /// The units animator component.
        /// </summary>
        public Animator Animator => _animator;

        /// <summary>
        /// The movement range of the unit.
        /// </summary>
        public MovementRange MovementRange => _movementRange;

        /// <summary>
        /// The animation manager for the unit.
        /// </summary>
        public UnitAnimationManager UnitAnimationManager => _unitAnimationManager;

        /// <summary>
        /// The stats manager for the unit.
        /// </summary>
        public UnitStatsManager UnitStatsManager => _unitStatsManager;

        /// <summary>
        /// The corresponding battle unit for the unit. 
        /// The battle unit is what is spawned during battle animations.
        /// </summary>
        public GameObject BattleUnit => _battleUnit;

        /// <summary>
        /// A bool indicating whether this unit is dead or not.
        /// </summary>
        public bool UnitDead => _unitDead;

        /// <summary>
        /// A bool indicating whether the unit has acted this turn.
        /// </summary>
        public bool HasActedThisTurn => _hasActedThisTurn;        

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _gridManager = _gameManager.GridManager;
            _turnManager = _gameManager.TurnManager;

            _stoodNode = DetectStoodNode();
            _stoodNode.StoodUnit = this;

            _updatedStoodNode = DetectStoodNode();
            _updatedStoodNode.StoodUnit = this;

            // TODO: Cleanup of gridcursor stuff
            _gridCursor = FindObjectOfType<GridCursor>();
        }

        private void Update()
        {
            if (IsPlayerUnit())
            {
                SetUnitInactiveState(_hasActedThisTurn);
            }

            UpdateTurnBasedState();
        }

        private void GetAllRenderers()
        {
            _allSMRRenderers = _modelBaseObject.GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
            _allMRRenderers = _modelBaseObject.GetComponentsInChildren<MeshRenderer>().ToList();

            _allRenderers.AddRange(_allSMRRenderers.Cast<Renderer>());
            _allRenderers.AddRange(_allMRRenderers.Cast<Renderer>());
        }

        // TODO: Cleanup this messy 'Inactive' setting
        private void SetUnitInactiveState(bool isInactive)
        {
            if (_allRenderers.Count == 0)
            {
                GetAllRenderers();
            }

            Material matToSet = isInactive ? _inactiveMaterial : _activeMaterial;

            foreach (Renderer renderer in _allRenderers)
            {
                renderer.material = matToSet;
            }
        }

        private NodeManager DetectStoodNode()
        {
            if (Physics.Raycast(transform.position, -transform.up, out _stoodNodeRayHit, 1))
            {
                return GetNodeFromRayHit(_stoodNodeRayHit);
            }
            else
            {
                UnityEngine.Debug.LogWarning("[WARNING]: Cast hit nothing");
                return null;
            }
        }

        private NodeManager GetNodeFromRayHit(RaycastHit hit)
        {
            if (hit.transform.gameObject.tag == NODE_TAG_REFERENCE)
            {
                return _stoodNodeRayHit.transform.parent.GetComponent<NodeManager>();
            }
            else
            {
                Debug.LogWarning("[ERROR]: Cast hit non-node object - " + _stoodNodeRayHit.transform.gameObject.name);
                return null;
            }
        }

        private void MoveToNextNode(Node endPoint)
        {
            var endPointPos = endPoint.UnitTransform.transform.position;

            var dir = (endPointPos - transform.position).normalized;
            var lookRot = Quaternion.LookRotation(dir);

            AdjustTransformValuesForNodeEndpoint(lookRot, endPoint);

            transform.DOMove(endPointPos, MOVEMENT_SPEED).SetEase(Ease.InOutQuad);
        }

        private void AdjustTransformValuesForNodeEndpoint(Quaternion lookRot, Node endPoint)
        {
            // Make the unit look toward where they're moving
            _modelBaseObject.transform.DORotateQuaternion(lookRot, LOOK_ROTATION_SPEED);

            // Adjust the unit's Y height if they go into a river
            if (endPoint.NodeManager.NodeData.TerrainType.TerrainType == Constants.Terrain.River)
            {
                _modelBaseObject.transform.DOLocalMoveY(UNIT_Y_VALUE_RIVER, UNIT_Y_ADJUSTMENT_SPEED);
            }
            else if (endPoint.NodeManager.NodeData.TerrainType.TerrainType == Constants.Terrain.Bridge)
            {
                _modelBaseObject.transform.DOLocalMoveY(UNIT_Y_VALUE_BRIDGE, UNIT_Y_ADJUSTMENT_SPEED);
            }
            else
            {
                _modelBaseObject.transform.DOLocalMoveY(UNIT_Y_VALUE_LAND, UNIT_Y_ADJUSTMENT_SPEED);
            }
        }

        private void ResetUnitState()
        {
            _gridManager.CurrentState = CurrentState.ActionSelected;

            _isMoving = false;
            _isSelected = false;
            _isInBattle = false;
            _gridCursor.UnitPressed = false;
            _gameManager.UnitsManager.SetActiveUnit(null);

            _gridManager.MovementPath.Clear();
        }

        private bool IsPlayerUnit()
        {
            return _unitData.UnitTeam == Team.Player;
        }

        /// <summary>
        /// Applies any terrain effects.
        /// </summary>
        public void ApplyTerrainEffects()
        {
            // We currently only have fort healing effects.
            if (StoodNode.NodeData.TerrainType.TerrainType == Constants.Terrain.Fort)
            {
                var percentage = StoodNode.NodeData.TerrainType.HealPercentageBoost;
                var healAmount = (UnitData.HealthPointsBaseValue * percentage) / 100;

                UnitStatsManager.AdjustHealthPoints(+healAmount);
            }
        }

        /// <summary>
        /// Updates the unit's ability to act based on the current turn phase.
        /// </summary>
        public void UpdateTurnBasedState()
        {
            if (_turnManager.ActivePhase == Phases.PlayerPhase && IsPlayerUnit())
            {
                EnableActions();
            }
            else
            {
                DisableActions();
            }
        }

        /// <summary>
        /// Enables the units actions for the current turn.
        /// </summary>
        private void EnableActions()
        {
            _canActThisTurn = true;
        }

        /// <summary>
        /// Disables the units actions outside of its turn.
        /// </summary>
        private void DisableActions()
        {
            _canActThisTurn = false;
        }

        /// <summary>
        /// Finalizes the unit's actions at the end of its turn.
        /// </summary>
        public void FinalizeTurn()
        {
            _hasActedThisTurn = true;
        }

        /// <summary>
        /// Resets the unit's turn.
        /// </summary>
        public void ResetTurn()
        {
            _hasActedThisTurn = false;
        }

        /// <summary>
        /// Handles the death of the unit.
        /// </summary>
        public void Death()
        {
            if (_unitData.UnitTeam == Team.Player)
            {
                _gameManager.PlayerDeaths += 1;
                _turnManager.TurnMusicManager.PlayDeathMusic();
            }
            
            ResetUnitState();

            _unitDead = true;

            ClearStoodUnit();
            _stoodNode.ClearStoodUnit();

            StartCoroutine(SetInactiveAfterDelay(4f));
        }


        /// <summary>
        /// Coroutine to set the game object inactive after a delay.
        /// </summary>
        /// <param name="delay">Time in seconds to wait before setting inactive.</param>
        private IEnumerator SetInactiveAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
            _gameManager.UnitsManager.UpdateAllUnits();
        }
        /// <summary>
        /// Finalizes the movement values of the unit after movement.
        /// </summary>
        public void FinalizeMovementValues(bool shouldFinalizeTurn = true)
        {
            ResetUnitState();

            _stoodNode = DetectStoodNode();
            _updatedStoodNode = _stoodNode;
            UpdateStoodNode(this);

            if (shouldFinalizeTurn)
            {
                FinalizeTurn();
            }
        }      

        /// <summary>
        /// Cancels the units movement.
        /// </summary>
        public void CancelMove()
        {
            _gridManager.CurrentState = CurrentState.ActionSelected;

            // Move the unit back to the original position
            StartCoroutine(MoveBackToPoint(_gridManager.MovementPath[0]));

            FinalizeMovementValues(false);
        }

        /// <summary>
        /// Updates the stood unit value from the node.
        /// </summary>
        /// <param name="unit">The unit you want to update the stood node of.</param>
        public void UpdateStoodNode(UnitManager unit)
        {
            if (_updatedStoodNode != null)
            {
                _updatedStoodNode.StoodUnit = unit;
            }

            if (_stoodNode != null)
            {
                _stoodNode.StoodUnit = unit;
            }
        }

        /// <summary>
        /// Clears the stood unit value from the node.
        /// This is used when a unit moves spaces.
        /// </summary>
        public void ClearStoodUnit()
        {
            if (_updatedStoodNode != null)
            {
                _updatedStoodNode.StoodUnit = null;
            }

            if (_stoodNode != null)
            {
                _stoodNode.StoodUnit = null;
            }
        }

        /// <summary>
        /// Move the unit to a specific node if it's within range.
        /// </summary>
        /// <param name="targetNode">The node to move to.</param>
        public bool MoveUnitToNode(Node targetNode, bool isAttacking)
        {
            if (IsNodeWithinRange(targetNode))
            {
                StartCoroutine(MoveToEndPoint(targetNode, isAttacking));
                return true;
            }
            else
            {
                Debug.LogWarning("[AI]: Target node is out of range");                
                return false;
            }
        }

        /// <summary>
        /// Move the unit to a specific node if it's within range.
        /// </summary>
        /// <param name="targetNode">The node to move to.</param>
        public bool MoveUnitToNode(Node targetNode, UnitAIManager ai, bool isAttacking)
        {
            if (IsNodeWithinRange(targetNode))
            {
                StartCoroutine(MoveToEndPoint(targetNode, ai, isAttacking));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Move the unit to a specific node if it's within range.
        /// </summary>
        /// <param name="targetNode">The node to move to.</param>
        public bool MoveUnitToNode(Node targetNode, bool shouldFinalize, bool isAttacking, UnitAIManager ai)
        {
            if (IsNodeWithinRange(targetNode))
            {
                StartCoroutine(MoveToEndPoint(targetNode, ai, isAttacking, shouldFinalize));
                return true;
            }
            else
            {
                Debug.LogWarning("[AI]: Target node is out of range");
                return false;
            }
        }

        /// <summary>
        /// Checks if a given node is within the movement range of the unit.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns>True if the node is within range, false otherwise.</returns>
        private bool IsNodeWithinRange(Node node)
        {
            return _movementRange.ReachableNodes.Contains(node);                        
        }

        /// <summary>
        /// Coroutine to move the unit to a node.
        /// </summary>
        /// <param name="targetNode">The target node to move to.</param>
        /// <returns></returns>
        private IEnumerator MoveToEndPoint(Node targetNode, bool isAttacking)
        {
            _isMoving = true;
            _gridManager.CurrentState = CurrentState.Moving;

            if (_stoodNode.Node != targetNode)
            {
                List<Node> path = MovementRange.ReconstructPath(_stoodNode.Node, targetNode);
                ClearStoodUnit();

                for (int i = 1; i < path.Count; i++)
                {
                    Node node = path[i];
                    MoveToNextNode(node);
                    _gridCursor.MoveCursorTo(node);
                    _updatedStoodNode = DetectStoodNode();                    
                    yield return new WaitForSeconds(MOVEMENT_DELAY);

                    if (i == path.Count - 1)
                    {
                        _isAwaitingMoveConfirmation = true;
                        _gridManager.CurrentState = CurrentState.ConfirmingMove;
                    }
                }

                if (!isAttacking)
                {
                    FinalizeMovementValues();
                }
                
            }
            else
            {
                Debug.LogError("[AI]: Start & End node are causing issues.");
            }         
        }

        /// <summary>
        /// Coroutine to move the unit to a node.
        /// </summary>
        /// <param name="targetNode">The target node to move to.</param>
        /// <returns></returns>
        private IEnumerator MoveToEndPoint(Node targetNode, UnitAIManager ai, bool isAttacking, bool shouldFinalize = true)
        {
            _isMoving = true;
            _gridManager.CurrentState = CurrentState.Moving;

            if (_stoodNode.Node != targetNode)
            {
                List<Node> path = MovementRange.ReconstructPath(_stoodNode.Node, targetNode);
                ClearStoodUnit();

                for (int i = 1; i < path.Count; i++)
                {
                    Node node = path[i];
                    MoveToNextNode(node);
                    _gridCursor.MoveCursorTo(node);
                    _updatedStoodNode = DetectStoodNode();
                    yield return new WaitForSeconds(MOVEMENT_DELAY);

                    if (i == path.Count - 1)
                    {
                        _isAwaitingMoveConfirmation = true;
                        _gridManager.CurrentState = CurrentState.ConfirmingMove;
                    }
                }

                ai.IsMoving = false;
                if (shouldFinalize && !isAttacking)
                {
                    FinalizeMovementValues();
                }                
            }
            else
            {
                Debug.LogError("[AI]: Start & End node are causing issues.");
            }
        }

        /// <summary>
        /// Coroutine to move the unit to a node.
        /// </summary>
        /// <param name="targetNode">The target node to move to.</param>
        /// <returns></returns>
        private IEnumerator MoveToEndPoint(Node targetNode, UnitAIManager ai, bool isAttacking)
        {
            _isMoving = true;
            _gridManager.CurrentState = CurrentState.Moving;

            if (_stoodNode.Node != targetNode)
            {
                List<Node> path = MovementRange.ReconstructPath(_stoodNode.Node, targetNode);
                ClearStoodUnit();

                for (int i = 1; i < path.Count; i++)
                {
                    Node node = path[i];
                    MoveToNextNode(node);
                    _gridCursor.MoveCursorTo(node);
                    _updatedStoodNode = DetectStoodNode();
                    yield return new WaitForSeconds(MOVEMENT_DELAY);

                    if (i == path.Count - 1)
                    {
                        _isAwaitingMoveConfirmation = true;
                        _gridManager.CurrentState = CurrentState.ConfirmingMove;
                    }
                }

                ai.IsMoving = false;

                if (!isAttacking)
                {
                    FinalizeMovementValues();
                }                                
            }
            else
            {
                Debug.LogError("[AI]: Start & End node are causing issues.");

                ai.IsMoving = false;

                if (!isAttacking)
                {
                    FinalizeMovementValues();
                }
            }
        }

        /// <summary>
        /// Move the unit to their selected end point
        /// </summary>
        /// <param name="modificationAmount">A value to take away from the end of the movement path. 
        /// Used for stopping before going into an enemy unit's node when battling.</param>
        /// <returns></returns>
        public IEnumerator MoveToEndPoint(int modificationAmount = 0)
        {
            _isMoving = true;
            _gridManager.CurrentState = CurrentState.Moving;

            for (int i = 1; i < _gridManager.MovementPath.Count - modificationAmount; i++)
            {
                Node n = _gridManager.MovementPath[i];

                MoveToNextNode(n);

                _updatedStoodNode = DetectStoodNode();

                yield return new WaitForSeconds(MOVEMENT_DELAY);

                if (i == (_gridManager.MovementPath.Count - 1) - modificationAmount)
                {
                    _isAwaitingMoveConfirmation = true;
                    _gridManager.CurrentState = CurrentState.ConfirmingMove;
                }
            }
        }

        /// <summary>
        /// Move the unit instantly to an end point. Used for when cancelling movement.
        /// </summary>
        public IEnumerator MoveBackToPoint(Node targetNode)
        {
            var endPointPos = targetNode.UnitTransform.transform.position;

            transform.position = endPointPos;

            var dir = (endPointPos - transform.position).normalized;
            var lookRot = Quaternion.LookRotation(dir);

            AdjustTransformValuesForNodeEndpoint(lookRot, targetNode);

            yield return new WaitForSeconds(MOVEMENT_DELAY_CANCEL);
        }
    }
}