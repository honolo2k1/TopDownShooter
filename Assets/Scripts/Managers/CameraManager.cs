using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer transposer;

    [Header("Camera Distance")]
    [SerializeField] private bool canChangeCameraDistance;
    [SerializeField] private float distanceChangeRate;
    [SerializeField] private float targetCameraDistance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }
    private void Update()
    {
        UpdateCameraDistance();
    }

    private void UpdateCameraDistance()
    {
        if (canChangeCameraDistance == false) return;

        float currentCameraDistance = transposer.m_CameraDistance;
        if (Mathf.Abs(targetCameraDistance - currentCameraDistance) < 0.01f)
        {
            return;
        }
        transposer.m_CameraDistance = Mathf.Lerp(currentCameraDistance, targetCameraDistance, distanceChangeRate * Time.deltaTime);
    }

    public void ChangeCameraDistance(float distance, float newChangeRate = 0.25f)
    {
        distanceChangeRate = newChangeRate;
        targetCameraDistance = distance;
    }

    public void ChangeCameraTarget(Transform target, float cameraDisance = 40, float newLookAheadTime = 0)
    {
        virtualCamera.Follow = target;
        transposer.m_LookaheadTime = newLookAheadTime;
        ChangeCameraDistance(cameraDisance);
    }
}
