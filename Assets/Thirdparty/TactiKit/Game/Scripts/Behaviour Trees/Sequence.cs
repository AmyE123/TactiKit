namespace CT6GAMAI.BehaviourTrees
{
    using System.Collections.Generic;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// Represents a Sequence node in a behaviour tree.
    /// A Sequence evaluates child nodes one after the other. 
    /// If one child succeeds, we move to the next, and if the next one fails, the selector node returns fail.
    /// All children must succeed in a sequence to return success.
    /// </summary>
    public class Sequence : BTNode
    {
        /// <summary>
        /// The child nodes/actions/sequences for this Sequence.
        /// </summary>
        private List<BTNode> _childNodes;

        private UnitAIManager _unitAI;

        private string _name;

        /// <summary>
        /// Index of the current node being evaluated.
        /// </summary>
        private int _currentNodeIndex;

        /// <summary>
        /// Initializes the sequence class with its children.
        /// </summary>
        /// <param name="childNodes">The nodes which will be evaluated.</param>
        public Sequence(List<BTNode> childNodes, UnitAIManager unitAI, string name)
        {
            _childNodes = childNodes;
            _unitAI = unitAI;
            _name = name;
            _currentNodeIndex = 0;
        }

        /// <summary>
        /// Evaluates each child node in order in the sequence. 
        /// The sequence returns RUNNING if any child node is still running, 
        /// SUCCESS if all child nodes succeed, and FAILURE if any child node fails.
        /// </summary>
        /// <returns>The evaluation state of the Sequence (RUNNING, SUCCESS, or FAILURE).</returns>
        public override BTNodeState Evaluate()
        {
            _unitAI.UpdateDebugActiveSequenceUI(_name);

            while (_currentNodeIndex < _childNodes.Count)
            {
                BTNode currentNode = _childNodes[_currentNodeIndex];
                BTNodeState nodeState = currentNode.Evaluate();

                _unitAI.UpdateDebugActiveActionStateUI(nodeState);

                if (nodeState == BTNodeState.RUNNING)
                {
                    _btNodeState = BTNodeState.RUNNING;
                    return _btNodeState;
                }

                if (nodeState == BTNodeState.FAILURE)
                {
                    _currentNodeIndex = 0;  // Reset for the next evaluation.
                    _btNodeState = BTNodeState.FAILURE;
                    return _btNodeState;
                }

                // Move to the next node.
                _currentNodeIndex++;
            }

            _currentNodeIndex = 0; // Reset for the next evaluation.
            _btNodeState = BTNodeState.SUCCESS;
            return _btNodeState;
        }
    }
}