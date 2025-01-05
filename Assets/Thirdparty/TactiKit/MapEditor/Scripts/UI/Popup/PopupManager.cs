namespace TactiKit.MapEditor
{
    using DG.Tweening;
    using UnityEngine;

    /// <summary>
    /// A manager for popups in the tool.
    /// </summary>
    public class PopupManager : MonoBehaviour
    {
        private bool _isPopupVisible = false;
        private const float HIDDEN_Y_POS = -500f;
        private const float VISIBLE_Y_POS = 0f;

        [SerializeField] private RectTransform _rectTransform;

        [Header("Animation Properties")]
        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private Ease _easeIn = Ease.InSine;
        [SerializeField] private Ease _easeOut = Ease.OutCubic;

        /// <summary>
        /// Returns whether this popup is visible or hidden.
        /// </summary>
        public bool IsPopupVisible => _isPopupVisible;

        private void Start()
        {
            if (_rectTransform == null)
            {
                Debug.LogWarning("[TACTIKIT/MapEditor] RectTransform for popup: '" + gameObject.name + "' has not been set in the inspector. Please set this value.");
                _rectTransform = GetComponent<RectTransform>();
            }
        }

        /// <summary>
        /// This toggles the popup between visible/hidden.
        /// </summary>
        public void TogglePopup()
        {
            SetPopupVisibility(!_isPopupVisible);
        }

        /// <summary>
        /// This is a function to force-show the popup if you don't want to use the toggle function.
        /// </summary>
        public void ShowPopup()
        {
            if (!_isPopupVisible)
            {
                SetPopupVisibility(true);
            }
        }

        /// <summary>
        /// This is a function to force-hide the popup if you don't want to use the toggle function.
        /// </summary>
        public void HidePopup()
        {
            if (_isPopupVisible)
            {
                SetPopupVisibility(false);
            }
        }

        private void SetPopupVisibility(bool visible)
        {
            float targetYPos = visible ? VISIBLE_Y_POS : HIDDEN_Y_POS;
            Ease popupEase = visible ? _easeOut : _easeIn;
            _rectTransform.DOAnchorPosY(targetYPos, _animationDuration).SetEase(popupEase);
            _isPopupVisible = visible;
        }
    }
}