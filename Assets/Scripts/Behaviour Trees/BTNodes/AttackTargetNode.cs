namespace CT6GAMAI.BehaviourTrees
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// 'attackTargetNode' node on the behaviour tree.
    /// This node handles logic for battle animations with the unit and target and waiting for completion.
    /// </summary>
    public class AttackTargetNode : BTNode
    {
        private UnitAIManager _unitAI;
        private bool _initiatedAttack = false;

        /// <summary>
        /// Initializes the AttackTargetNode class with a reference to the AI manager.
        /// </summary>
        /// <param name="unitAI">The AI which is attacking.</param>
        public AttackTargetNode(UnitAIManager unitAI)
        {
            _unitAI = unitAI;
        }

        /// <summary>
        /// Evaluates the node by initiating an attack and waiting for its completion.
        /// The node returns RUNNING while the attack is animating and SUCCESS once the attack is completed.
        /// </summary>
        /// <returns>The state of the node after evaluation.</returns>
        public override BTNodeState Evaluate()
        {
            _unitAI.UpdateDebugActiveActionUI(GetType().Name);

            if (!_initiatedAttack)
            {
                _unitAI.AttackTargetUnit();
                _initiatedAttack = true;

                return BTNodeState.RUNNING;
            }

            else
            {
                if (_unitAI.UnitManager.IsInBattle)
                {
                    if (_unitAI.UnitManager.UnitDead)
                    {
                        return BTNodeState.SUCCESS;
                    }

                    return BTNodeState.RUNNING;
                }
                else
                {
                    _initiatedAttack = false;

                    return BTNodeState.SUCCESS;
                }
            }
        }
    }
}