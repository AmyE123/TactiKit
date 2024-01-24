namespace CT6GAMAI.BehaviourTrees
{
    using static CT6GAMAI.Constants;

    // -- START -- The following code snippet was adapted from GameDevChef, 2021 (https://youtu.be/F-3nxJ2ANXg?si=YNF0_0i24xuX5wRh)
    /// <summary>
    /// This represents a single node in a behaviour tree.
    /// </summary>
    [System.Serializable]
    public abstract class BTNode
    {
        /// <summary>
        /// The current state of the node.
        /// </summary>
        protected BTNodeState _btNodeState;

        /// <summary>
        /// A getter for the current state of the node.
        /// </summary>
        public BTNodeState BTNodeState => _btNodeState;

        /// <summary>
        /// An evaluate method which should be implemented by node classes with the specific behaviours.
        /// </summary>
        /// <returns>The state after evaluation.</returns>
        public abstract BTNodeState Evaluate();
    }
    // -- END -- The following code snippet was adapted from GameDevChef, 2021 (https://youtu.be/F-3nxJ2ANXg?si=YNF0_0i24xuX5wRh)
}