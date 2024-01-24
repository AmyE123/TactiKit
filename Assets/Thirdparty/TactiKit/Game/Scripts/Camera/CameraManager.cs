namespace CT6GAMAI
{
    using UnityEngine;
    using Cinemachine;
    using static CT6GAMAI.Constants;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;
    using DG.Tweening;

    /// <summary>
    /// Manages camera switching and states for map view and battle view.
    /// </summary>
    public class CameraManager : MonoBehaviour
    {
        [Header("Camera Configuration")]
        [SerializeField] private CameraStates _cameraState;
        [SerializeField] private CinemachineVirtualCamera[] _cameras;
        [SerializeField] private Volume _mainCameraVolume;
        [SerializeField] private Camera _uiCamera;

        [Header("Camera Settings")]
        [SerializeField] private CinemachineVirtualCamera _currentCamera;
        [SerializeField] private bool _isMapCamZoomedOut = false;

        [Header("Camera Types")]
        [SerializeField] private CinemachineVirtualCamera[] _mapCameras;
        [SerializeField] private CinemachineVirtualCamera _battleCamera;

        /// <summary>
        /// Gets an array of all CinemachineVirtualCameras.
        /// </summary>
        public CinemachineVirtualCamera[] Cameras => _cameras;

        /// <summary>
        /// Gets the current state of the camera.
        /// </summary>
        public CameraStates CameraState => _cameraState;

        private void Start()
        {
            SetActiveCamera(_mapCameras[0]);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                ToggleMapCameraZoom();
            }
        }

        private void ToggleMapCameraZoom()
        {
            if (CameraState == CameraStates.Map)
            {
                if (_isMapCamZoomedOut)
                {
                    SwitchCamera(_mapCameras[0]);
                }
                else
                {
                    SwitchCamera(_mapCameras[1]);
                }

                _isMapCamZoomedOut = !_isMapCamZoomedOut;
            }
            else
            {
                _isMapCamZoomedOut = false;
            }
        }

        private void SetUICameraSettings(CinemachineVirtualCamera activeCamera, float duration)
        {
            _uiCamera.DOFieldOfView(activeCamera.m_Lens.FieldOfView, duration).SetEase(Ease.InOutSine);

            PaniniProjection paniniProjection;
            if (_mainCameraVolume.profile.TryGet(out paniniProjection))
            {
                float targetDistance = activeCamera == _mapCameras[1] ? 0 : 1;
                DOVirtual.Float(paniniProjection.distance.value, targetDistance, duration, value =>
                {
                    paniniProjection.distance.value = value;
                }).SetEase(Ease.InOutSine);
            }
        }

        private void SetActiveCamera(CinemachineVirtualCamera newCamera)
        {
            if (newCamera == null || newCamera == _currentCamera) return;

            if (_currentCamera != null)
            {
                _currentCamera.Priority = INACTIVE_CAMERA_PRIORITY;
            }

            _currentCamera = newCamera;
            _currentCamera.Priority = ACTIVE_CAMERA_PRIORITY;
        }

        /// <summary>
        /// Switches the camera.
        /// </summary>
        /// <param name="newCamera">The new camera to be activated.</param>
        public void SwitchCamera(CinemachineVirtualCamera newCamera)
        {
            SetActiveCamera(newCamera);
            SetUICameraSettings(newCamera, 0.2f);

            if (newCamera == _mapCameras[0] || newCamera == _mapCameras[1])
            {
                _cameraState = CameraStates.Map;
            }
            if (newCamera == _battleCamera)
            {
                _cameraState = CameraStates.Battle;
                _isMapCamZoomedOut = false;
            }
        }
    }
}