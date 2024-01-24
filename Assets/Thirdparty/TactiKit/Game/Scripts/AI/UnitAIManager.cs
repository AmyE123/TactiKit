namespace CT6GAMAI
{
    using CT6GAMAI.BehaviourTrees;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using static CT6GAMAI.Constants;

    public class UnitAIManager : MonoBehaviour
    {
        [SerializeField] private UnitManager _unitManager;

        [Header("AI Information")]
        [SerializeField] private UnitManager _targetUnit;
        [SerializeField] private bool _nextTargetOverride = false;

        /// <summary>
        /// A boolean for the AI to toggle when moving within the behaviour tree.
        /// </summary>
        public bool IsMoving;

        [Header("Utility Theory Information")]
        [SerializeField] private AIPlaystyleWeighting _playstyle;
        [SerializeField] private int _unitPowerAmount;
        [SerializeField] private int _unitCurrentHealth;
        [SerializeField] private List<VisibleUnitDetails> _visibleUnitsDetails = new List<VisibleUnitDetails>();
        [SerializeField] private List<VisibleTerrainDetails> _visibleUniqueTerrainDetails = new List<VisibleTerrainDetails>();

        [Header("Utility Theory Desirabilities")]
        [SerializeField] private int _fortDesirability;
        [SerializeField] private int _retreatDesirability;
        [SerializeField] private int _attackDesirability;
        [SerializeField] private int _waitDesirability;

        private BTNode _topNode;
        private GameManager _gameManager;
        private UnitStatsManager _unitStatsManager;

        /// <summary>
        /// An override boolean indicating whether we have to get the next target if the current one is unavaliable
        /// </summary>
        public bool NextTargetOverride { get { return _nextTargetOverride; } set { _nextTargetOverride = value; } }

        /// <summary>
        /// A boolean used for attacking AI
        /// </summary>
        public bool HasAttacked { get; set; } = false;

        /// <summary>
        /// The AI's playstyle. Aggressive, Normal, Easy, etc.
        /// </summary>
        public AIPlaystyleWeighting Playstyle => _playstyle;

        /// <summary>
        /// The current health this unit has. Useful when determining desirabilities based on health.
        /// </summary>
        public int UnitCurrentHealth => _unitCurrentHealth;

        /// <summary>
        /// The unit which this AI unit is currently targetting. This is always updated even if the unit doesn't have a desire to attack.
        /// </summary>
        public UnitManager TargetUnit => _targetUnit;

        /// <summary>
        /// A reference to unit manager.
        /// </summary>
        public UnitManager UnitManager => _unitManager;

        /// <summary>
        /// A reference to unit stats.
        /// </summary>
        public UnitStatsManager UnitStatsManager => _unitStatsManager;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _unitPowerAmount = BattleCalculator.CalculatePower(_unitManager);
            _unitStatsManager = _unitManager.UnitStatsManager;

            ConstructBehaviourTree();
        }

        private void Update()
        {
            _unitCurrentHealth = _unitManager.UnitStatsManager.HealthPoints;
            GetBestUnitToAttack();
        }

        private void ConstructBehaviourTree()
        {
            MoveToSafeSpaceNode moveToSafeSpaceNode = new MoveToSafeSpaceNode(this);
            MoveToAttackPositionNode moveToAttackPositionNode = new MoveToAttackPositionNode(this);
            MoveToFortNode moveToFortNode = new MoveToFortNode(this);
            AttackTargetNode attackTargetNode = new AttackTargetNode(this);
            WaitNode waitNode = new WaitNode(this);
            DefaultNode defaultNode = new DefaultNode(this);

            CheckAttackDesirabilityNode checkAttackDesirabilityNode = new CheckAttackDesirabilityNode(this);
            CheckFortDesirabilityNode checkFortDesirabilityNode = new CheckFortDesirabilityNode(this);
            CheckRetreatDesirabilityNode checkRetreatDesirabilityNode = new CheckRetreatDesirabilityNode(this);
            CheckWaitDesirabilityNode checkWaitDesirabilityNode = new CheckWaitDesirabilityNode(this);

            Sequence waitSequence = new Sequence(new List<BTNode> { checkWaitDesirabilityNode, waitNode }, this, "Wait Sequence");
            Sequence attackSequence = new Sequence(new List<BTNode> { checkAttackDesirabilityNode, moveToAttackPositionNode, attackTargetNode }, this, "Attack Sequence");
            Sequence fortSequence = new Sequence(new List<BTNode> { checkFortDesirabilityNode, moveToFortNode }, this, "Fort Sequence");
            Sequence retreatSequence = new Sequence(new List<BTNode> { checkRetreatDesirabilityNode, moveToSafeSpaceNode }, this, "Retreat Sequence");
            Sequence defaultSequence = new Sequence(new List<BTNode> { defaultNode }, this, "Default Sequence");

            _topNode = new Selector(new List<BTNode> { retreatSequence, fortSequence, attackSequence, waitSequence, defaultSequence });
        }

        private IEnumerator EnemyAITurn()
        {
            HasAttacked = false;
            _unitManager.IsInBattle = false;
            yield return new WaitForSeconds(2);

            GetVisiblePlayerUnits();
            GetVisibleUniqueTerrain();

            _fortDesirability = AIDesirabilityCalculator.CalculateFortDesirability(this);
            _retreatDesirability = AIDesirabilityCalculator.CalculateRetreatDesirability(this);
            _attackDesirability = AIDesirabilityCalculator.CalculateAttackDesirability(this);
            _waitDesirability = AIDesirabilityCalculator.CalculateWaitDesirability(this);

            _gameManager.UIManager.UI_DebugDesirabilityManager.PopulateDebugDesirability(this);

            UpdateDebugActiveAIUI(_unitManager.UnitData.UnitName);

            // Loop to continuously evaluate the behaviour tree
            BTNodeState state = _topNode.Evaluate();

            while (state == BTNodeState.RUNNING)
            {
                yield return null; // Wait for the next frame
                state = _topNode.Evaluate(); // Re-evaluate the tree

                if (_unitManager.UnitDead)
                {
                    yield return new WaitForSeconds(3);
                    state = BTNodeState.SUCCESS;
                }
            }
        }

        private List<VisibleUnitDetails> GetVisiblePlayerUnits()
        {
            var range = _unitManager.MovementRange.ReachableNodes;
            _visibleUnitsDetails.Clear();

            foreach (Node node in range)
            {
                var stoodUnit = node.NodeManager.StoodUnit;
                if (stoodUnit != null)
                {
                    if (stoodUnit.UnitData.UnitTeam == Constants.Team.Player)
                    {
                        _visibleUnitsDetails.Add(new VisibleUnitDetails(stoodUnit, GetDistanceToUnit(stoodUnit)));
                    }
                }
            }

            return _visibleUnitsDetails;
        }

        private List<VisibleTerrainDetails> GetVisibleUniqueTerrain()
        {
            var range = _unitManager.MovementRange.ReachableNodes;
            _visibleUniqueTerrainDetails.Clear();

            foreach (Node node in range)
            {
                var terrainType = node.NodeManager.NodeData.TerrainType.TerrainType;

                if (!(terrainType == Constants.Terrain.Plain || terrainType == Constants.Terrain.Unwalkable))
                {
                    _visibleUniqueTerrainDetails.Add(new VisibleTerrainDetails(node.NodeManager, GetDistanceToNode(node.NodeManager)));
                }
            }

            return _visibleUniqueTerrainDetails;
        }

        private int GetDistanceToUnit(UnitManager unit)
        {
            Node startNode = _unitManager.StoodNode.Node;
            Node targetNode = unit.StoodNode.Node;

            var path = _unitManager.MovementRange.ReconstructPath(startNode, targetNode);
            return path.Count;
        }

        private int GetDistanceToNode(NodeManager node)
        {
            Node startNode = _unitManager.StoodNode.Node;
            Node targetNode = node.Node;

            var path = _unitManager.MovementRange.ReconstructPath(startNode, targetNode);
            return path.Count;
        }

        private float GetDistanceToNearestUniqueTerrainType(Constants.Terrain terrainTarget)
        {
            List<VisibleTerrainDetails> allTerrain = GetVisibleUniqueTerrain();
            List<VisibleTerrainDetails> uniqueTerrain = new List<VisibleTerrainDetails>();

            foreach (VisibleTerrainDetails terrainDetails in allTerrain)
            {
                Constants.Terrain terrainType = terrainDetails.TerrainNode.NodeData.TerrainType.TerrainType;
                if (terrainType == terrainTarget)
                {
                    uniqueTerrain.Add(terrainDetails);
                }
            }

            float closestDistance = Constants.MAX_NODE_COST;

            foreach (VisibleTerrainDetails terrain in uniqueTerrain)
            {
                if (terrain.Distance < closestDistance)
                {
                    closestDistance = terrain.Distance;
                }
            }

            return closestDistance;
        }

        private Node GetNearestUniqueTerrainNode(Constants.Terrain terrainTarget)
        {
            List<VisibleTerrainDetails> allTerrain = GetVisibleUniqueTerrain();
            List<VisibleTerrainDetails> uniqueTerrain = new List<VisibleTerrainDetails>();
            Node closestNode = null;

            foreach (VisibleTerrainDetails terrainDetails in allTerrain)
            {
                Constants.Terrain terrainType = terrainDetails.TerrainNode.NodeData.TerrainType.TerrainType;
                if (terrainType == terrainTarget)
                {
                    uniqueTerrain.Add(terrainDetails);
                }
            }

            float closestDistance = MAX_NODE_COST;

            foreach (VisibleTerrainDetails terrain in uniqueTerrain)
            {
                if (terrain.Distance < closestDistance && terrain.TerrainNode.Node.NodeManager.StoodUnit == null)
                {
                    closestDistance = terrain.Distance;
                    closestNode = terrain.TerrainNode.Node;
                }
            }

            return closestNode;
        }

        private int GetDistanceToUnit(UnitManager unit, Node fromNode)
        {
            Node startNode = fromNode;
            Node targetNode = unit.StoodNode.Node;

            var path = _unitManager.MovementRange.ReconstructPath(startNode, targetNode);
            return path.Count;
        }

        /// <summary>
        /// Begins the AI turn for the enemy.
        /// </summary>
        public IEnumerator BeginEnemyAI()
        {
            _gameManager.GridManager.GridCursor.MoveCursorTo(_unitManager.StoodNode.Node);
            yield return StartCoroutine(EnemyAITurn());
        }

        /// <summary>
        /// Changes the playstyle of the AI.
        /// </summary>
        /// <param name="newPlaystyle">The new playstyle to be set.</param>
        public void ChangePlaystyle(Playstyle newPlaystyle)
        {
            foreach (AIPlaystyleWeighting playstyle in _gameManager.AIManager.Playstyles)
            {
                if (playstyle.Playstyle == newPlaystyle)
                {
                    _playstyle = playstyle;
                }
            }
        }

        /// <summary>
        /// Updates the UI with the current active action in debug mode.
        /// </summary>
        /// <param name="activeAction">The active action to display.</param>
        public void UpdateDebugActiveActionUI(string activeAction)
        {
            _gameManager.UIManager.UI_DebugBehaviourTree.UpdateActiveAction(activeAction);
        }

        /// <summary>
        /// Updates the UI with the current state of the active action in debug mode.
        /// </summary>
        /// <param name="activeActionState">The state of the active action to display.</param>
        public void UpdateDebugActiveActionStateUI(BTNodeState activeActionState)
        {
            _gameManager.UIManager.UI_DebugBehaviourTree.UpdateActiveActionState(activeActionState);
        }

        /// <summary>
        /// Updates the UI with the current active sequence in debug mode.
        /// </summary>
        /// <param name="activeSequence">The active sequence to display.</param>
        public void UpdateDebugActiveSequenceUI(string activeSequence)
        {
            _gameManager.UIManager.UI_DebugBehaviourTree.UpdateActiveSequence(activeSequence);
        }

        /// <summary>
        /// Updates the UI with the current active AI unit in debug mode.
        /// </summary>
        /// <param name="activeAI">The active AI unit to display.</param>
        public void UpdateDebugActiveAIUI(string activeAI)
        {
            _gameManager.UIManager.UI_DebugBehaviourTree.UpdateActiveAIUnit(activeAI);
        }

        /// <summary>
        /// Executes the wait action for the unit.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool Wait()
        {
            _unitManager.FinalizeMovementValues(true);
            return true;
        }

        /// <summary>
        /// Performs an attack on the targeted unit.
        /// </summary>
        /// <returns>True if the attack was executed, otherwise false.</returns>
        public bool AttackTargetUnit()
        {
            if (_targetUnit != null && !HasAttacked)
            {
                _unitManager.IsInBattle = true;

                _gameManager.BattleManager.CalculateValuesForBattleForecast(_targetUnit, _unitManager);
                _gameManager.BattleManager.SwitchToBattle(Team.Enemy);

                HasAttacked = true;

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the distance to the best safe spot from enemies.
        /// </summary>
        /// <returns>The distance to the safest spot.</returns>
        public float GetDistanceToBestSafeSpot()
        {
            GetVisiblePlayerUnits();
            List<Node> movementRange = _unitManager.MovementRange.ReachableNodes;
            float maxMinDistanceToPlayer = 0;
            Node furthestNode = null;

            foreach (Node node in movementRange)
            {
                float minDistanceToPlayer = float.MaxValue;

                foreach (VisibleUnitDetails playerUnitDetails in _visibleUnitsDetails)
                {
                    float distanceToPlayer = GetDistanceToUnit(playerUnitDetails.Unit, node);

                    if (distanceToPlayer < minDistanceToPlayer)
                    {
                        minDistanceToPlayer = distanceToPlayer;
                    }
                }

                if (minDistanceToPlayer > maxMinDistanceToPlayer)
                {
                    maxMinDistanceToPlayer = minDistanceToPlayer;
                    furthestNode = node;
                }
            }

            return furthestNode != null ? maxMinDistanceToPlayer : -1;
        }

        /// <summary>
        /// Identifies the best safe spot from enemies.
        /// </summary>
        /// <returns>The node representing the safest spot.</returns>
        public Node GetBestSafeSpot()
        {
            GetVisiblePlayerUnits();
            List<Node> movementRange = _unitManager.MovementRange.ReachableNodes;
            float maxMinDistanceToPlayer = 0;
            Node furthestNode = null;

            foreach (Node node in movementRange)
            {
                float minDistanceToPlayer = float.MaxValue;

                foreach (VisibleUnitDetails playerUnitDetails in _visibleUnitsDetails)
                {
                    float distanceToPlayer = GetDistanceToUnit(playerUnitDetails.Unit, node);

                    if (distanceToPlayer < minDistanceToPlayer)
                    {
                        minDistanceToPlayer = distanceToPlayer;
                    }
                }

                if (minDistanceToPlayer > maxMinDistanceToPlayer && node.NodeManager.StoodUnit == null)
                {
                    maxMinDistanceToPlayer = minDistanceToPlayer;
                    furthestNode = node;
                }
            }

            return furthestNode;
        }

        /// <summary>
        /// Determines the highest desirability action for the unit.
        /// </summary>
        /// <returns>The action with the highest desirability.</returns>
        public Constants.Action GetHighestDesirabilityAction()
        {
            int maxDesirability = _fortDesirability;
            List<Constants.Action> actions = new List<Constants.Action> { Constants.Action.Fort };

            // Check each desirability, and if it's equal to the current max, add it to the list.
            // This accounts for when 2 desirabilities values are the same but they're the highest number still.
            // If it's greater, reset the list and update the maxDesirability.
            if (_retreatDesirability > maxDesirability)
            {
                actions.Clear();
                maxDesirability = _retreatDesirability;
                actions.Add(Constants.Action.Retreat);
            }
            else if (_retreatDesirability == maxDesirability)
            {
                actions.Add(Constants.Action.Retreat);
            }

            if (_attackDesirability > maxDesirability)
            {
                actions.Clear();
                maxDesirability = _attackDesirability;
                actions.Add(Constants.Action.Attack);
            }
            else if (_attackDesirability == maxDesirability)
            {
                actions.Add(Constants.Action.Attack);
            }

            if (_waitDesirability > maxDesirability)
            {
                actions.Clear();
                maxDesirability = _waitDesirability;
                actions.Add(Constants.Action.Wait);
            }
            else if (_waitDesirability == maxDesirability)
            {
                actions.Add(Constants.Action.Wait);
            }

            // If there's a tie, choose an action at random from the list of highest actions.
            if (actions.Count > 1)
            {
                int randomIndex = UnityEngine.Random.Range(0, actions.Count);
                return actions[randomIndex];
            }

            // If there's no tie, return the single highest action.
            return actions[0];
        }

        /// <summary>
        /// Gets the distance to the nearest fort to the Unit.
        /// </summary>
        /// <returns>The distance to the nearest fort.</returns>
        public float GetDistanceToNearestFort()
        {
            return GetDistanceToNearestUniqueTerrainType(Constants.Terrain.Fort);
        }

        /// <summary>
        /// Finds the nearest fort to the unit.
        /// </summary>
        /// <returns>The nearest fort node.</returns>
        public Node GetNearestFort()
        {
            return GetNearestUniqueTerrainNode(Constants.Terrain.Fort);
        }

        /// <summary>
        /// Gets the distance to the nearest player to the Unit.
        /// </summary>
        /// <returns>The distance to the nearest player.</returns>
        public float GetDistanceToNearestPlayer()
        {
            List<VisibleUnitDetails> allUnits = GetVisiblePlayerUnits();

            float closestDistance = Constants.MAX_NODE_COST;

            foreach (VisibleUnitDetails unit in allUnits)
            {
                if (unit.Distance < closestDistance)
                {
                    closestDistance = unit.Distance;
                }
            }

            return closestDistance;
        }

        /// <summary>
        /// Identifies the best unit to attack based on the current situation.
        /// </summary>
        /// <returns>The best unit for the AI to attack.</returns>
        public UnitManager GetBestUnitToAttack()
        {
            List<VisibleUnitDetails> allUnits = GetVisiblePlayerUnits();
            List<VisibleUnitDetails> hurtUnits = new List<VisibleUnitDetails>();
            bool isAnyUnitHurt = false;

            foreach (VisibleUnitDetails unit in allUnits)
            {
                var unitStats = unit.Unit.UnitStatsManager;

                if (unitStats.HealthPoints < unitStats.UnitBaseData.HealthPointsBaseValue)
                {
                    hurtUnits.Add(unit);
                    isAnyUnitHurt = true;
                }
            }

            if (!_nextTargetOverride)
            {

                if (isAnyUnitHurt)
                {
                    _targetUnit = GetNearestPlayer(hurtUnits);
                }
                else
                {
                    _targetUnit = GetNearestPlayer();
                }
            }

            return _targetUnit;
        }

        /// <summary>
        /// Finds the nearest player unit to the AI.
        /// </summary>
        /// <returns>The nearest player unit.</returns>
        public UnitManager GetNearestPlayer()
        {
            List<VisibleUnitDetails> allUnits = GetVisiblePlayerUnits();
            UnitManager closestUnit = null;

            float closestDistance = MAX_NODE_COST;

            foreach (VisibleUnitDetails unit in allUnits)
            {
                if (unit.Distance < closestDistance)
                {
                    closestUnit = unit.Unit;
                }
            }

            return closestUnit;
        }

        /// <summary>
        /// Finds the nearest player unit from a specified list of units.
        /// </summary>
        /// <param name="units">The list of units to consider.</param>
        /// <returns>The nearest player unit from the specified list.</returns>
        public UnitManager GetNearestPlayer(List<VisibleUnitDetails> units)
        {
            UnitManager closestUnit = null;

            float closestDistance = MAX_NODE_COST;

            foreach (VisibleUnitDetails unit in units)
            {
                if (unit.Distance < closestDistance)
                {
                    closestUnit = unit.Unit;
                }
            }

            return closestUnit;
        }

        /// <summary>
        /// Finds the next target for the AI.
        /// </summary>
        /// <returns>The next target unit, or null if no suitable target is found.</returns>
        public UnitManager FindNextTarget()
        {
            if (_visibleUnitsDetails.Count > 1)
            {
                foreach (VisibleUnitDetails unit in _visibleUnitsDetails)
                {
                    if (unit.Unit != _targetUnit)
                    {
                        _nextTargetOverride = true;
                        _targetUnit = unit.Unit;
                        return _targetUnit;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if the unit can move to a spot to attack the target.
        /// </summary>
        /// <param name="target">The target unit.</param>
        /// <returns>True if the unit can move to an attack spot, otherwise false.</returns>
        public bool CanMoveToTargetAttackSpot(UnitManager target)
        {
            List<Node> movementRange = _unitManager.MovementRange.ReachableNodes;
            List<Node> targetAttackSpots = new List<Node>();
            List<Node> validAttackSpots = new List<Node>();

            if (target.StoodNode.NorthNode != null)
            {
                targetAttackSpots.Add(target.StoodNode.NorthNode.Node);
            }

            if (target.StoodNode.EastNode != null)
            {
                targetAttackSpots.Add(target.StoodNode.EastNode.Node);
            }

            if (target.StoodNode.SouthNode != null)
            {
                targetAttackSpots.Add(target.StoodNode.SouthNode.Node);
            }

            if (target.StoodNode.WestNode != null)
            {
                targetAttackSpots.Add(target.StoodNode.WestNode.Node);
            }

            foreach (Node n in targetAttackSpots)
            {
                if (n == _unitManager.StoodNode.Node)
                {
                    validAttackSpots.Add(n);
                }

                if (movementRange.Contains(n))
                {
                    if (n.NodeManager.StoodUnit == null)
                    {
                        validAttackSpots.Add(n);
                    }
                    else
                    {
                        if (n.NodeManager.StoodUnit == this)
                        {
                            validAttackSpots.Add(n);
                        }
                    }
                }
            }

            return validAttackSpots.Count > 0;
        }

        /// <summary>
        /// Finds a valid attack spot for the player.
        /// </summary>
        /// <param name="player">The player unit.</param>
        /// <returns>The node representing a valid attack spot for the player.</returns>
        public Node GetPlayerValidAttackSpot(UnitManager player)
        {
            List<Node> movementRange = _unitManager.MovementRange.ReachableNodes;
            List<Node> attackSpots = new List<Node>();
            List<Node> validAttackSpots = new List<Node>();

            if (player.StoodNode.NorthNode != null)
            {
                attackSpots.Add(player.StoodNode.NorthNode.Node);
            }

            if (player.StoodNode.EastNode != null)
            {
                attackSpots.Add(player.StoodNode.EastNode.Node);
            }

            if (player.StoodNode.SouthNode != null)
            {
                attackSpots.Add(player.StoodNode.SouthNode.Node);
            }

            if (player.StoodNode.WestNode != null)
            {
                attackSpots.Add(player.StoodNode.WestNode.Node);
            }

            foreach (Node n in attackSpots)
            {
                if (n == _unitManager.StoodNode.Node)
                {
                    return n;
                }

                if (movementRange.Contains(n))
                {
                    if (n.NodeManager.StoodUnit == null)
                    {
                        validAttackSpots.Add(n);
                    }
                    else
                    {
                        if (n.NodeManager.StoodUnit == this)
                        {
                            return n;
                        }
                    }
                }
            }

            return validAttackSpots[UnityEngine.Random.Range(0, validAttackSpots.Count)];
        }

        /// <summary>
        /// Checks if any player units are visible to the AI.
        /// </summary>
        /// <returns>True if any players are visible, otherwise false.</returns>
        public bool ArePlayersVisible()
        {
            GetVisiblePlayerUnits();
            return _visibleUnitsDetails.Count > 0;
        }

        /// <summary>
        /// Calculates the power advantage over an opponent.
        /// </summary>
        /// <param name="opponent">The opponent unit.</param>
        /// <returns>The calculated power advantage.</returns>
        public float CalculatePowerAdvantage(UnitManager opponent)
        {
            int opponentPower = BattleCalculator.CalculatePower(opponent);
            int unitPower = BattleCalculator.CalculatePower(_unitManager, opponent);
            float powerAdvantage = unitPower - opponentPower;

            // Normalize power advantage, assuming a maximum expected power difference
            float maxPowerDifference = 50;
            powerAdvantage = Mathf.Clamp(powerAdvantage, -maxPowerDifference, maxPowerDifference);
            powerAdvantage = (powerAdvantage + maxPowerDifference) / (2 * maxPowerDifference);

            return powerAdvantage;
        }

        /// <summary>
        /// Initiates the movement of the unit towards a target node.
        /// </summary>
        /// <param name="targetNode">The target node.</param>
        /// <param name="isAttacking">Indicates whether the move is part of an attack.</param>
        /// <returns>True if the movement is initiated, otherwise false.</returns>
        public bool StartMovingTo(Node targetNode, bool isAttacking)
        {
            IsMoving = true;
            return _unitManager.MoveUnitToNode(targetNode, this, isAttacking);
        }
    }

    /// <summary>
    /// Represents the details of a visible unit, including the unit itself and its distance.
    /// </summary>
    [Serializable]
    public class VisibleUnitDetails
    {
        /// <summary>
        /// The unit that is visible.
        /// </summary>
        public UnitManager Unit;

        /// <summary>
        /// The distance to the visible unit.
        /// </summary>
        public float Distance;

        /// <summary>
        /// Initializes a new instance of the VisibleUnitDetails class.
        /// </summary>
        /// <param name="unit">The visible unit.</param>
        /// <param name="distance">The distance to the unit.</param>
        public VisibleUnitDetails(UnitManager unit, float distance)
        {
            Unit = unit;
            Distance = distance;
        }
    }

    /// <summary>
    /// Represents the details of visible terrain, including the terrain node and its distance.
    /// </summary>
    [Serializable]
    public class VisibleTerrainDetails
    {
        /// <summary>
        /// The terrain node that is visible.
        /// </summary>
        public NodeManager TerrainNode;

        /// <summary>
        /// The distance to the visible terrain node.
        /// </summary>
        public float Distance;

        /// <summary>
        /// Initializes a new instance of the VisibleTerrainDetails class.
        /// </summary>
        /// <param name="terrainNode">The visible terrain node.</param>
        /// <param name="distance">The distance to the terrain node.</param>
        public VisibleTerrainDetails(NodeManager terrainNode, float distance)
        {
            TerrainNode = terrainNode;
            Distance = distance;
        }
    }
}