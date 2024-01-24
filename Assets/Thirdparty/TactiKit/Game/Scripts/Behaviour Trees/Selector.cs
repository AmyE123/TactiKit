namespace CT6GAMAI.BehaviourTrees
{
    using System.Collections.Generic;
    using static CT6GAMAI.Constants;

    // -- START -- The following code snippet was adapted from GameDevChef, 2021 (https://youtu.be/F-3nxJ2ANXg?si=YNF0_0i24xuX5wRh)
    /// <summary>
    /// Represents a Selector node in a behaviour tree.
    /// A selector evaluates child nodes one after the other. 
    /// If one child fails, we move to the next, and if the next one succeeds, the selector node returns successful.
    /// A single child must succeed in a selector to return success.
    /// </summary>
    public class Selector : BTNode
    {
        /// <summary>
        /// The child nodes/actions/sequences for this Selector.
        /// </summary>
        private List<BTNode> _childNodes = new List<BTNode>();

        /// <summary>
        /// Initializes the selector class with its children.
        /// </summary>
        /// <param name="childNodes">The nodes which will be evaluated.</param>
        public Selector(List<BTNode> childNodes)
        {
            _childNodes = childNodes;
        }

        /// <summary>
        /// Evaluates each child node in order. Returns SUCCESS if any child node succeeds,
        /// RUNNING if any child node is still running, and FAILURE if all child nodes fail.
        /// </summary>
        /// <returns>The state of the Selector after evaluating its child nodes.</returns>
        public override BTNodeState Evaluate()
        {
            foreach (BTNode node in _childNodes)
            {
                switch (node.Evaluate())
                {
                    case BTNodeState.RUNNING:
                        _btNodeState = BTNodeState.RUNNING;
                        return _btNodeState;
                    case BTNodeState.SUCCESS:
                        _btNodeState = BTNodeState.SUCCESS;
                        return _btNodeState;
                    case BTNodeState.FAILURE:
                        break;
                    default:
                        break;
                }
            }

            _btNodeState = BTNodeState.FAILURE;
            return _btNodeState;
        }
    }
    // -- END -- The following code snippet was adapted from GameDevChef, 2021 (https://youtu.be/F-3nxJ2ANXg?si=YNF0_0i24xuX5wRh)
}