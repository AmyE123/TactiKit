namespace CT6GAMAI
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// A Finite-State Machine to control the visual state of a node
    /// </summary>
    public class NodeVisualFSM
    {
        private NodeVisualState _currentState;

        private NodeVisualManager _manager;

        /// <summary>
        /// The visual manager for the node
        /// </summary>
        public NodeVisualManager Manager
        {
            get { return _manager; }
            set { _manager = value; }
        }

        /// <summary>
        /// A constructor for the NodeVisualFSM
        /// </summary>
        public NodeVisualFSM()
        {
            _currentState = NodeVisualState.Default;
        }

        /// <summary>
        /// Change the state of the node visual.
        /// </summary>
        /// <param name="newState">The state to change to.</param>
        public void ChangeState(NodeVisualState newState)
        {
            if (_currentState == newState)
                return;

            // Update the node's appearance based on the new state
            switch (newState)
            {
                case NodeVisualState.Default:
                    _manager.State.ChangeVisualData(_manager.VisualSR, _manager.VisualDatas[0]);
                    break;

                case NodeVisualState.HoveredBlue:
                    _manager.State.ChangeVisualData(_manager.VisualSR, _manager.VisualDatas[1]);
                    break;

                case NodeVisualState.HoveredRed:
                    _manager.State.ChangeVisualData(_manager.VisualSR, _manager.VisualDatas[2]);
                    break;

                case NodeVisualState.HoveredGreen:
                    _manager.State.ChangeVisualData(_manager.VisualSR, _manager.VisualDatas[3]);
                    break;

                case NodeVisualState.SelectedBlue:
                    _manager.State.ChangeVisualData(_manager.VisualSR, _manager.VisualDatas[4]);
                    break;

                case NodeVisualState.SelectedRed:
                    _manager.State.ChangeVisualData(_manager.VisualSR, _manager.VisualDatas[5]);
                    break;

                case NodeVisualState.SelectedGreen:
                    _manager.State.ChangeVisualData(_manager.VisualSR, _manager.VisualDatas[6]);
                    break;

                case NodeVisualState.AllEnemyRange:
                    _manager.State.ChangeVisualData(_manager.VisualSR, _manager.VisualDatas[7]);
                    break;

                case NodeVisualState.SingularEnemyRange:
                    _manager.State.ChangeVisualData(_manager.VisualSR, _manager.VisualDatas[8]);
                    break;

                case NodeVisualState.PointOfInterest:
                    _manager.State.ChangeVisualData(_manager.VisualSR, _manager.VisualDatas[9]);
                    break;
            }

            _currentState = newState;
        }
    }
}