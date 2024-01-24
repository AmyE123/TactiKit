namespace CT6GAMAI.BehaviourTrees
{
    using static CT6GAMAI.Constants;

    /// <summary>
    /// 'moveToAttackPositionNode' node on the behaviour tree.
    /// This node moves the unit to an appropriate position to attack a target.
    /// </summary>
    public class MoveToAttackPositionNode : BTNode
    {
        private UnitAIManager _unitAI;
        private bool _initiatedMovement = false;
        private Node _attackPosition;

        /// <summary>
        /// Initializes the MoveToAttackPositionNode class with a reference to the AI manager.
        /// </summary>
        /// <param name="unitAI">The AI which is attacking.</param>
        public MoveToAttackPositionNode(UnitAIManager unitAI)
        {
            _unitAI = unitAI;
        }

        /// <summary>
        /// Moves towards an attack position whilst evaluating the states.
        /// The node checks if the unit can move to a target attack spot and then moves.
        /// If the target's attack spots are full, we find another target with a valid attack spot.
        /// </summary>
        /// <returns>
        /// Returns RUNNING while the unit is in the process of moving to the attack position.
        /// Once the unit reaches the position, it returns SUCCESS.
        /// </returns>
        public override BTNodeState Evaluate()
        {
            _unitAI.UpdateDebugActiveActionUI(GetType().Name);

            if (!_initiatedMovement)
            {
                if (_unitAI.CanMoveToTargetAttackSpot(_unitAI.TargetUnit))
                {
                    _unitAI.NextTargetOverride = false;
                    _attackPosition = _unitAI.GetPlayerValidAttackSpot(_unitAI.TargetUnit);
                    _unitAI.StartMovingTo(_attackPosition, true);
                    _initiatedMovement = true;
                }
                else
                {
                    var nextTarget = _unitAI.FindNextTarget();
                    if (nextTarget != null)
                    {
                        if (_unitAI.CanMoveToTargetAttackSpot(nextTarget))
                        {
                            _attackPosition = _unitAI.GetPlayerValidAttackSpot(nextTarget);
                            _unitAI.StartMovingTo(_attackPosition, true);
                            _initiatedMovement = true;
                        }
                    }
                    else
                    {
                        return BTNodeState.FAILURE;
                    }
                }

                return BTNodeState.RUNNING;
            }
            else if (_unitAI.IsMoving)
            {
                // Still moving towards the target.
                return BTNodeState.RUNNING;
            }
            else
            {
                // Movement completed and unit is on the attack position.
                _initiatedMovement = false; // Reset for the next time this node is evaluated
                return BTNodeState.SUCCESS;
            }
        }
    }
}