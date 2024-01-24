namespace CT6GAMAI.BehaviourTrees
{
    using static CT6GAMAI.Constants;

    /// <summary>
    /// 'checkWaitDesirabilityNode' node on the behaviour tree.
    /// This node checks if the most desirable action for a unit is to wait.
    /// </summary>
    public class CheckWaitDesirabilityNode : BTNode
    {
        private UnitAIManager _unitAI;

        /// <summary>
        /// Initializes the CheckWaitDesirabilityNode class with a reference to the AI manager.
        /// </summary>
        /// <param name="unitAI">The AI which is attacking.</param>
        public CheckWaitDesirabilityNode(UnitAIManager unitAI)
        {
            _unitAI = unitAI;
        }

        /// <summary>
        /// Evaluates the node by checking whether waiting is the highest desirability action for the unit.
        /// Returns SUCCESS if waiting is the most desirable action, otherwise it returns FAILURE.
        /// </summary>
        /// <returns>The state of the node after evaluation - SUCCESS if waiting is desirable, otherwise FAILURE.</returns>
        public override BTNodeState Evaluate()
        {
            _unitAI.UpdateDebugActiveActionUI(GetType().Name);
            return _unitAI.GetHighestDesirabilityAction() == Action.Wait ? BTNodeState.SUCCESS : BTNodeState.FAILURE;
        }
    }
}