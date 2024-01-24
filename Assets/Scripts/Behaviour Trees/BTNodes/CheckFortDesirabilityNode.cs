namespace CT6GAMAI.BehaviourTrees
{
    using static CT6GAMAI.Constants;

    /// <summary>
    /// 'checkFortDesirabilityNode' node on the behaviour tree.
    /// This node checks if the most desirable action for a unit is to find a fort to heal in.
    /// </summary>
    public class CheckFortDesirabilityNode : BTNode
    {
        private UnitAIManager _unitAI;

        /// <summary>
        /// Initializes the CheckFortDesirabilityNode class with a reference to the AI manager.
        /// </summary>
        /// <param name="unitAI">The AI which is attacking.</param>
        public CheckFortDesirabilityNode(UnitAIManager unitAI)
        {
            _unitAI = unitAI;
        }

        /// <summary>
        /// Evaluates the node by checking whether going to a fort is the highest desirability action for the unit.
        /// Returns SUCCESS if going to a fort is the most desirable action, otherwise it returns FAILURE.
        /// </summary>
        /// <returns>The state of the node after evaluation - SUCCESS if forts are desirable, otherwise FAILURE.</returns>
        public override BTNodeState Evaluate()
        {
            _unitAI.UpdateDebugActiveActionUI(GetType().Name);
            return _unitAI.GetHighestDesirabilityAction() == Action.Fort ? BTNodeState.SUCCESS : BTNodeState.FAILURE;
        }
    }
}