namespace CT6GAMAI.BehaviourTrees
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// 'waitNode' node on the behaviour tree.
    /// This node makes the unit end their turn without moving.
    /// </summary>
    public class WaitNode : BTNode
    {
        private UnitAIManager _unitAI;

        /// <summary>
        /// Initializes the WaitNode class with a reference to the AI manager.
        /// </summary>
        /// <param name="unitAI">The AI which is attacking.</param>
        public WaitNode(UnitAIManager unitAI)
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