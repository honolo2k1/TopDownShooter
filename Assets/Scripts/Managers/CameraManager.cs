using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoSingleton<CameraManager>
{
    private CinemachineCamera virtualCamera;
    private CinemachinePositionComposer positionComposer;

    [Header("Camera Distance")]
    [SerializeField] private bool canChangeCameraDistance;
    [SerializeField] private float distanceChangeRate;
    [SerializeField] private float targetCameraDistance;

    override protected void Initiate()
    {

        virtualCamera = GetComponentInChildren<CinemachineCamera>();
        positionComposer = virtualCamera.GetComponent<CinemachinePositionComposer>();
    }

    private void Update()
    {
        UpdateCameraDistance();
    }

    private void UpdateCameraDistance()
    {
        if (canChangeCameraDistance == false)
        {
            return;
        }

        float currentCameraDistance = positionComposer.CameraDistance;
        if (Mathf.Abs(targetCameraDistance - currentCameraDistance) < 0.01f)
        {
            return;
        }

        positionComposer.CameraDistance = Mathf.Lerp(currentCameraDistance, targetCameraDistance, distanceChangeRate * Time.deltaTime);
    }

    public void ChangeCameraDistance(float distance, float newChangeRate = 0.25f)
    {
        distanceChangeRate = newChangeRate;
        targetCameraDistance = distance;
    }

    public void ChangeCameraTarget(Transform target, float cameraDistance = 15, float newLookAheadTime = 0)
    {
        virtualCamera.Follow = target;

        var lookahead = positionComposer.Lookahead;
        lookahead.Time = newLookAheadTime;
        positionComposer.Lookahead = lookahead;

        ChangeCameraDistance(cameraDistance);
    }
}