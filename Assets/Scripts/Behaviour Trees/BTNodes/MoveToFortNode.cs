namespace CT6GAMAI.BehaviourTrees
{
    using static CT6GAMAI.Constants;

    /// <summary>
    /// 'moveToFortNode' node on the behaviour tree.
    /// This node moves the unit to a nearby fort.
    /// </summary>
    public class MoveToFortNode : BTNode
    {
        private UnitAIManager _unitAI;
        private bool _initiatedMovement = false;

        /// <summary>
        /// Initializes the MoveToFortNode class with a reference to the AI manager.
        /// </summary>
        /// <param name="unitAI">The AI which is attacking.</param>
        public MoveToFortNode(UnitAIManager unitAI)
        {
            _unitAI = unitAI;
        }

        /// <summary>
        /// Moves to a fort position whilst evaluating the states.
        /// </summary>
        /// <returns>
        /// Once the unit reaches the fort position, it returns SUCCESS.
        /// If this isn't able to be done, return FAILURE.
        /// </returns>
        public override BTNodeState Evaluate()
        {
            _unitAI.UpdateDebugActiveActionUI(GetType().Name);

            if (!_initiatedMovement)
            {
                bool canMoveToFort = _unitAI.StartMovingTo(_unitAI.GetNearestFort(), false);

                if (canMoveToFort)
                {
                    _initiatedMovement = true;
                    return BTNodeState.RUNNING;
                }
                else
                {
                    // The unit is most likely already on the fort, so you'd want them to wait anyway.
                    return BTNodeState.FAILURE;
                }
            }
            else if (_unitAI.IsMoving)
            {
                return BTNodeState.RUNNING;
            }
            else
            {
                _initiatedMovement = false;
                return BTNodeState.SUCCESS;
            }
        }
    }
}