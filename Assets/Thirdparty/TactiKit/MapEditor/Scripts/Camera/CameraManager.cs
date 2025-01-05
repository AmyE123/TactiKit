namespace TactiKit.MapEditor
{
    using Cinemachine;
    using UnityEngine;
    using UnityEngine.UI;

    public class CameraManager : MonoBehaviour
    {
        private CinemachineVirtualCamera _activeCamera;

        [SerializeField] private GridSpawner _gridSpawner;

        [SerializeField] private GameObject _freeFlyCameraGO;
        [SerializeField] private CinemachineVirtualCamera _freeFlyCamera;

        [SerializeField] private GameObject _birdsEyeViewCameraGO;
        [SerializeField] private CinemachineVirtualCamera _birdsEyeViewCamera;

        [SerializeField] private Toggle _cameraTypeToggle;

        [SerializeField] private float _sensitivity = 10f;
        [SerializeField] private float _minFOV = 15f;
        [SerializeField] private float _maxFOV = 90f;

        private void Start()
        {
            _freeFlyCameraGO.SetActive(false);
            _birdsEyeViewCameraGO.SetActive(true);
        }

        private void Update()
        {
            _activeCamera = _birdsEyeViewCameraGO.activeInHierarchy ? _birdsEyeViewCamera : _freeFlyCamera;

            float scroll = Input.GetAxis("Mouse ScrollWheel");

            _activeCamera.m_Lens.FieldOfView -= scroll * _sensitivity;
            _activeCamera.m_Lens.FieldOfView = Mathf.Clamp(_activeCamera.m_Lens.FieldOfView, _minFOV, _maxFOV);
        }

        public void SwitchCameras()
        {
            _freeFlyCameraGO.SetActive(!_freeFlyCameraGO.activeSelf);
            _birdsEyeViewCameraGO.SetActive(!_birdsEyeViewCameraGO.activeSelf);

            _cameraTypeToggle.isOn = _freeFlyCameraGO.activeInHierarchy;
        }

        public void ResetCameras()
        {
            _birdsEyeViewCamera.m_Lens.FieldOfView = 70f;
            _freeFlyCamera.m_Lens.FieldOfView = 70f;

            RecenterCamera(_birdsEyeViewCamera);
            RecenterCamera(_freeFlyCamera);
        }

        private void RecenterCamera(CinemachineVirtualCamera camera)
        {
            Vector3 centerPosition = _gridSpawner.CenterOfGrid;

            camera.transform.position = centerPosition + new Vector3(0, 10, 0);
            camera.transform.LookAt(centerPosition);
        }
    }
}