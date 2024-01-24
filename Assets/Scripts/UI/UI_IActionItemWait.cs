namespace CT6GAMAI
{
    using UnityEngine;

    public class UI_IActionItemWait : ActionItemBase
    {
        [SerializeField] private RectTransform _pointer;
        [SerializeField] private Actions _action = Actions.Wait;
        [SerializeField] private bool _isActive = false;
        [SerializeField] private bool _isSelected = false;

        private GameManager _gameManager;
        private UIManager _uiManager;
        private GlobalUnitsManager _unitsManager;
        private GridManager _gridManager;

        public override Actions Action
        {
            get { return _action; }
            set { _action = value; }
        }

        public override bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        public override bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }

        public override RectTransform Pointer => _pointer;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _unitsManager = _gameManager.UnitsManager;
            _uiManager = _gameManager.UIManager;
            _gridManager = _gameManager.GridManager;


            AnimatePointer();
        }

        public override void ActionEvent()
        {
            var unit = _unitsManager.ActiveUnit;
            unit.FinalizeMovementValues();
            unit.IsAwaitingMoveConfirmation = false;
            _uiManager.ActionItemsManager.HideActionItems();
        }
    }
}