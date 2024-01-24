namespace CT6GAMAI
{
    using UnityEngine;

    /// <summary>
    /// Manages the animations for a unit.
    /// </summary>
    public class UnitAnimationManager : MonoBehaviour
    {
        [SerializeField] private bool _ignoreAnimationWarning = false;
        [SerializeField] private UnitManager _unitManager;
        [SerializeField] private Animator _animator;

        [SerializeField] private GridCursor _gridCursor;

        private bool _warningLogSent = false;        

        private void Update()
        {
            if (_animator != null)
            {
                UpdateAnimationParameters();
            }
            else
            {
                if(!_warningLogSent && !_ignoreAnimationWarning)
                {
                    TryForceAnimationComponentAttach();
                }
            }
        }

        /// <summary>
        /// Tries to attach an Animator component to the unit if one is not already attached.
        /// </summary>
        private void TryForceAnimationComponentAttach()
        {
            _animator = GetComponentInChildren<Animator>();
            if (_animator != null)
            {
                Debug.LogWarning("[UNIT]: " + gameObject.name + " Has no Animator attached, please attach this in the inspector. " +
                    "Attached " + _animator.name + "'s animation in it's place.");
            }
            else
            {
                Debug.LogWarning("[UNIT]: " + gameObject.name + " Has no Animator component in children.");
            }
            _warningLogSent = true;
        }

        private void UpdateAnimationParameters()
        {
            _animator.SetBool(Constants.MOVING_ANIM_PARAM, _unitManager.IsMoving);
            _animator.SetBool(Constants.READY_ANIM_PARAM, _unitManager.IsSelected);
        }
    }
}