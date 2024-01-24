namespace CT6GAMAI
{
    using static CT6GAMAI.Constants;

    /// <summary>
    /// A Finite-State Machine to control the cursor state of a node
    /// </summary>
    public class NodeCursorFSM
    {
        private NodeCursorState _currentState;

        private NodeCursorManager _manager;

        /// <summary>
        /// The cursor manager for this cursor
        /// </summary>
        public NodeCursorManager Manager
        {
            get { return _manager; }
            set { _manager = value; }
        }

        /// <summary>
        /// Constructor for the NodeCursorFSM
        /// </summary>
        public NodeCursorFSM()
        {
            _currentState = NodeCursorState.NoSelection;
        }

        /// <summary>
        /// Gets the current state of the cursor.
        /// </summary>
        /// <returns>The current state of the cursor.</returns>
        public NodeCursorState GetState()
        {
            return _currentState;
        }

        /// <summary>
        /// Change the state of the cursor.
        /// </summary>
        /// <param name="newState">The state to change to.</param>
        public void ChangeState(NodeCursorState newState)
        {
            if (_currentState == newState)
                return;

            // Update the node's appearance based on the new state
            // TODO: Add more visual datas for each state
            switch (newState)
            {
                case NodeCursorState.NoSelection:
                    _manager.State.ChangeVisualData(_manager.CursorSR, _manager.CursorVisualDatas[1]);
                    break;
                case NodeCursorState.DefaultSelected:
                    _manager.State.ChangeVisualData(_manager.CursorSR, _manager.CursorVisualDatas[0]);
                    break;

                case NodeCursorState.PlayerSelected:
                    _manager.State.ChangeVisualData(_manager.CursorSR, _manager.CursorVisualDatas[0]);
                    break;

                case NodeCursorState.EnemySelected:
                    _manager.State.ChangeVisualData(_manager.CursorSR, _manager.CursorVisualDatas[0]);
                    break;
            }

            _currentState = newState;
        }
    }
}