namespace TactiKit
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(GridLayoutGroup))]
    public class UI_GridLayoutFitContent : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup _gridLayoutGroup;
        [SerializeField] private RectTransform _rectTransform;

        private void Start()
        {
            if (_gridLayoutGroup == null)
            {
                Debug.LogWarning("[TACTIKIT/MapEditor] A 'GridLayoutGroup' reference has not been set in the inspector. Please set this value.");
                _gridLayoutGroup = GetComponent<GridLayoutGroup>();
            }

            if (_rectTransform == null)
            {
                Debug.LogWarning("[TACTIKIT/MapEditor] A 'RectTransform' reference has not been set in the inspector. Please set this value.");
                _rectTransform = GetComponent<RectTransform>();
            }
        }

        private void Update()
        {
            int childCount = transform.childCount;
            int columns = Mathf.FloorToInt(_rectTransform.rect.width / (_gridLayoutGroup.cellSize.x + _gridLayoutGroup.spacing.x));
            int rows = Mathf.CeilToInt((float)childCount / columns);
            float totalHeight = rows * _gridLayoutGroup.cellSize.y + (rows - 1) * _gridLayoutGroup.spacing.y + _gridLayoutGroup.padding.top + _gridLayoutGroup.padding.bottom;

            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, totalHeight);
        }
    }
}