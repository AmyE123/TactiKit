namespace CT6GAMAI.BehaviourTrees
{
    using static CT6GAMAI.Constants;

    /// <summary>
    /// 'checkRetreatDesirabilityNode' node on the behaviour tree.
    /// This node checks if the most desirable action for a unit is to retreat.
    /// </summary>
    public class CheckRetreatDesirabilityNode : BTNode
    {
        private UnitAIManager _unitAI;

        /// <summary>
        /// Initializes the CheckRetreatDesirabilityNode class with a reference to the AI manager.
        /// </summary>
        /// <param name="unitAI">The AI which is attacking.</param>
        public CheckRetreatDesirabilityNode(UnitAIManager unitAI)
        {
            _unitAI = unitAI;
        }

        /// <summary>
        /// Evaluates the node by checking whether retreating is the highest desirability action for the unit.
        /// Returns SUCCESS if retreating is the most desirable action, otherwise it returns FAILURE.
        /// </summary>
        /// <returns>The state of the node after evaluation - SUCCESS if retreating is desirable, otherwise FAILURE.</returns>
        public override BTNodeState Evaluate()
        {
            _unitAI.UpdateDebugActiveActionUI(GetType().Name);
            return _unitAI.GetHighestDesirabilityAction() == Action.Retreat ? BTNodeState.SUCCESS : BTNodeState.FAILURE;
        }
    }
}