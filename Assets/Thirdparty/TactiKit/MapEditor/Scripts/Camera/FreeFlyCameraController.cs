namespace TactiKit.MapEditor
{
    using UnityEngine;
    using Cinemachine;

    public class FreeFlyCameraController : MonoBehaviour
    {
        private CinemachineVirtualCamera _virtualCamera;
        private Transform _cameraTransform;

        [SerializeField] private float _movementSpeed = 10f;
        [SerializeField] private float _rotationSpeed = 100f;

        void Start()
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            _cameraTransform = _virtualCamera.VirtualCameraGameObject.transform;
        }

        void Update()
        {
            // Check for middle mouse button press
            if (Input.GetMouseButton(2))
            {
                // Rotate camera based on mouse movement
                float mouseX = Input.GetAxis("Mouse X") * _rotationSpeed * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * _rotationSpeed * Time.deltaTime;

                _cameraTransform.Rotate(Vector3.up, mouseX, Space.World);
                _cameraTransform.Rotate(Vector3.right, -mouseY, Space.Self);
            }

            // Movement using WASD keys
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 movement = (_cameraTransform.forward * vertical + _cameraTransform.right * horizontal) * _movementSpeed * Time.deltaTime;
            _cameraTransform.position += movement;
        }
    }
}