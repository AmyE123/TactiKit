namespace CT6GAMAI.BehaviourTrees
{
    using static CT6GAMAI.Constants;

    /// <summary>
    /// 'defaultNode' node on the behaviour tree.
    /// This is the default move that will happen if all other sequences return failure.
    /// </summary>
    public class DefaultNode : BTNode
    {
        private UnitAIManager _unitAI;

        /// <summary>
        /// Initializes the DefaultNode class with a reference to the AI manager.
        /// </summary>
        /// <param name="unitAI">The AI which is attacking.</param>
        public DefaultNode(UnitAIManager unitAI)
        {
            _unitAI = unitAI;
        }

        /// <summary>
        /// Waits in position whilst evaluating the states.
        /// </summary>
        /// <returns>
        /// Once the unit waits, it returns SUCCESS.
        /// If this isn't able to be done, return FAILURE.
        /// </returns>
        public override BTNodeState Evaluate()
        {
            _unitAI.UpdateDebugActiveActionUI(GetType().Name);
            return _unitAI.Wait() ? BTNodeState.SUCCESS : BTNodeState.FAILURE;
        }
    }
}