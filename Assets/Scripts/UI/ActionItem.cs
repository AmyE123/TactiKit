namespace CT6GAMAI
{
    using DG.Tweening;
    using Unity.Burst.CompilerServices;
    using UnityEngine;

    public interface IActionItem
    {
        public RectTransform Pointer { get; }
        public void ActionEvent();
        public void AnimatePointer();
        public void SetActionItemActive(bool isActive);
    }

    public abstract class ActionItemBase : MonoBehaviour, IActionItem
    {
        public enum Actions { Attack, Wait }

        public abstract RectTransform Pointer { get; }

        public abstract bool IsActive { get; set; }
        public abstract bool IsSelected { get; set; }

        public abstract Actions Action { get; set; }

        public abstract void ActionEvent();

        public void AnimatePointer()
        {
            Pointer.DOAnchorPosX(0, Constants.POINTER_X_YOYO_SPEED).SetLoops(-1, LoopType.Yoyo);
        }

        public void SetActionItemActive(bool isActive)
        {
            IsActive = isActive;
        }

        public void SetActionItemSelected()
        {
            IsSelected = true;
            Pointer.gameObject.SetActive(true);
        }

        public void SetActionItemDisselected()
        {
            IsSelected = false;
            Pointer.gameObject.SetActive(false);
        }
    }
}