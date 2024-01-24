namespace TactiKit.MapEditor
{
    using UnityEngine;
    using DG.Tweening;

    /// <summary>
    /// A general manager for the toolbox.
    /// </summary>
    public class ToolboxManager : MonoBehaviour
    {
        private bool _isPopupVisible = false;
        private const float HIDDEN_Y_POS = -180f;
        private const float VISIBLE_Y_POS = 0f;
        private const float UP_ARROW_X_ROT = 180;

        [SerializeField] private GridSpawner _gridSpawner;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private RectTransform _arrowRT;

        [Header("Animation Properties")]
        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private Ease _easeIn = Ease.InSine;
        [SerializeField] private Ease _easeOut = Ease.OutCubic;

        /// <summary>
        /// Returns whether the toolbox is visible or hidden.
        /// </summary>
        public bool IsPopupVisible => _isPopupVisible;

        private void Start()
        {
            if (_rectTransform == null)
            {
                Debug.LogWarning("[TACTIKIT/MapEditor] RectTransform for 'Toolbox' has not been set in the inspector. Please set this value.");
                _rectTransform = GetComponent<RectTransform>();
            }
        }

        /// <summary>
        /// This toggles the toolbox between visible/hidden.
        /// </summary>
        public void ToggleToolbox()
        {
            if (_gridSpawner.MapSpawned)
            {
                SetPopupVisibility(!_isPopupVisible);
            }          
        }

        /// <summary>
        /// This is a function to force-show the toolbox if you don't want to use the toggle function.
        /// </summary>
        public void ShowToolbox()
        {
            if (!_isPopupVisible && _gridSpawner.MapSpawned)
            {
                SetPopupVisibility(true);
            }
        }

        /// <summary>
        /// This is a function to force-hide the toolbox if you don't want to use the toggle function.
        /// </summary>
        public void HideToolbox()
        {
            if (_isPopupVisible && _gridSpawner.MapSpawned)
            {
                SetPopupVisibility(false);
            }
        }

        private void SetPopupVisibility(bool visible)
        {
            float targetYPos = visible ? VISIBLE_Y_POS : HIDDEN_Y_POS;
            float targetArrowRot = visible ? UP_ARROW_X_ROT : 0;
            Ease popupEase = visible ? _easeOut : _easeIn;
            _rectTransform.DOAnchorPosY(targetYPos, _animationDuration).SetEase(popupEase);
            _arrowRT.DOLocalRotate(new Vector3(targetArrowRot, 0, 0), _animationDuration).SetEase(popupEase);
            _isPopupVisible = visible;
        }
    }

}