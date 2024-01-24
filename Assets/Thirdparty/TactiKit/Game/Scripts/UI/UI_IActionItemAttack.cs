namespace CT6GAMAI
{
    using UnityEngine;

    public class UI_IActionItemAttack : ActionItemBase
    {
        [SerializeField] private RectTransform _pointer;
        [SerializeField] private Actions _action = Actions.Attack;
        [SerializeField] private bool _isActive = false;
        [SerializeField] private bool _isSelected = false;

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
            AnimatePointer();
        }

        public override void ActionEvent()
        {
        }
    }
}