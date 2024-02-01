namespace TactiKit.MapEditor
{
    using UnityEngine;

    public class BirdsEyeViewCameraController : MonoBehaviour
    {
        [SerializeField] private float _panSpeed = 20f;
        [SerializeField] private Vector3 _dragOrigin;

        void Update()
        {
            if (Input.GetMouseButtonDown(2))
            {
                _dragOrigin = Input.mousePosition;
                return;
            }

            if (!Input.GetMouseButton(2)) return;

            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - _dragOrigin);
            Vector3 move = new Vector3(pos.x * _panSpeed, 0, pos.y * _panSpeed);

            transform.Translate(-move, Space.World);
            _dragOrigin = Input.mousePosition;
        }
    }
}