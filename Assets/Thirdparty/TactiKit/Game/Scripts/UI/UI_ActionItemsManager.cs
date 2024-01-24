namespace CT6GAMAI
{
    using System.Collections.Generic;
    using UnityEngine;

    public class UI_ActionItemsManager : MonoBehaviour
    {
        [SerializeField] private bool _isActionItemsActive;
        [SerializeField] private int _actionIndex = 0;

        [SerializeField] private GameObject _actionItemsGO;
        [SerializeField] private ActionItemBase[] _allActionItems;
        [SerializeField] private List<ActionItemBase> _activeActionItems = new List<ActionItemBase>();

        private GameManager _gameManager;
        private UIManager _uiManager;
        private GlobalUnitsManager _unitsManager;
        private GridManager _gridManager;

        public bool IsActionItemsActive => _isActionItemsActive;
        public ActionItemBase[] AllActionItems => _allActionItems;
        public List<ActionItemBase> ActiveActionItems => _activeActionItems;

        public void ShowActionItems(bool canAttack = false)
        {
            _isActionItemsActive = true;
            PopulateActionItems(canAttack);
            MakeActionItemsVisible();
            RefreshActionItemCursor();
        }

        public void HideActionItems()
        {
            _isActionItemsActive = false;
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _unitsManager = _gameManager.UnitsManager;
            _uiManager = _gameManager.UIManager;
            _gridManager = _gameManager.GridManager;
        }

        private void Update()
        {
            _actionItemsGO.SetActive(_isActionItemsActive);

            if (_isActionItemsActive)
            {
                if (_activeActionItems.Count <= 1)
                {
                    _actionIndex = 0;
                    RefreshActionItemCursor();
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        if (_actionIndex == 0)
                        {
                            _actionIndex = 1;
                            RefreshActionItemCursor();
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.W))
                    {
                        if (_actionIndex == 1)
                        {
                            _actionIndex = 0;
                            RefreshActionItemCursor();
                        }
                    }
                }

                var unit = _unitsManager.ActiveUnit;

                if (unit != null && unit.IsAwaitingMoveConfirmation)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        _activeActionItems[_actionIndex].ActionEvent();
                    }
                }
            }
        }

        private void PopulateActionItems(bool canAttack)
        {
            _activeActionItems.Clear();

            foreach (ActionItemBase actionItem in _allActionItems)
            {
                if (canAttack)
                {
                    if (actionItem.Action == ActionItemBase.Actions.Attack)
                    {
                        _activeActionItems.Add(actionItem);
                    }
                }
                if (actionItem.Action == ActionItemBase.Actions.Wait)
                {
                    _activeActionItems.Add(actionItem);
                }
            }
        }

        private void MakeActionItemsVisible()
        {
            foreach (ActionItemBase actionItem in _allActionItems)
            {                
                if (_activeActionItems.Contains(actionItem))
                {
                    actionItem.gameObject.SetActive(true);
                }
                else
                {
                    actionItem.gameObject.SetActive(false);
                }
            }
        }

        private void RefreshActionItemCursor()
        {
            for (int i = 0; i < _activeActionItems.Count; i++)
            {
                _activeActionItems[i].Pointer.gameObject.SetActive(false);

                if (i == _actionIndex)
                {
                    _activeActionItems[i].Pointer.gameObject.SetActive(true);
                }
                else
                {
                    _activeActionItems[i].Pointer.gameObject.SetActive(false);
                }
            }
        }
    }
}