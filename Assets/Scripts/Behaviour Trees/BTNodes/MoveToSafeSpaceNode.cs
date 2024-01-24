namespace CT6GAMAI.BehaviourTrees
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// 'moveToSafeSpaceNode' node on the behaviour tree.
    /// This node moves the unit to a nearby safe spot.
    /// </summary>
    public class MoveToSafeSpaceNode : BTNode
    {
        private UnitAIManager _unitAI;
        private bool _initiatedMovement = false;

        /// <summary>
        /// Initializes the MoveToSafeSpaceNode class with a reference to the AI manager.
        /// </summary>
        /// <param name="unitAI">The AI which is attacking.</param>
        public MoveToSafeSpaceNode(UnitAIManager unitAI)
        {
            _unitAI = unitAI;
        }

        /// <summary>
        /// Moves to a safe position whilst evaluating the states.
        /// </summary>
        /// <returns>
        /// Once the unit reaches the safe position, it returns SUCCESS.
        /// If this isn't able to be done, return FAILURE.
        /// </returns>
        public override BTNodeState Evaluate()
        {
            _unitAI.UpdateDebugActiveActionUI(GetType().Name);

            if (!_initiatedMovement)
            {
                bool canMoveToSafeSpace = _unitAI.StartMovingTo(_unitAI.GetBestSafeSpot(), false);

                if (canMoveToSafeSpace)
                {
                    _initiatedMovement = true;
                    return BTNodeState.RUNNING;
                }
                else
                {
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


            //return _unitAI.MoveUnitTo(_unitAI.GetBestSafeSpot(), false) ? BTNodeState.SUCCESS : BTNodeState.FAILURE;
        }
    }
}