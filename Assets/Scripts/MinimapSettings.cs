using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapSettings : MonoBehaviour
{

    public Camera MiniCamera;
    [SerializeField] private float _cameraHeight;
    [SerializeField] private Transform _targetToFollow;

    public float CameraHeight
    {
        set { _cameraHeight = value; }
        get { return _cameraHeight; }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FollowTarget();
    }

    public void InitMiniMap(float cameraHeight)
    {
        _cameraHeight = cameraHeight;
        _targetToFollow = GameObject.FindObjectOfType<Player>().transform;

    }
    private void FollowTarget()
    {
        Vector3 targetPosition = _targetToFollow.transform.position;
        MiniCamera.transform.position = new Vector3(targetPosition.x, _cameraHeight, targetPosition.z);
        Quaternion targetRotation = _targetToFollow.transform.rotation;
        //MiniCamera.transform.rotation = Quaternion.Euler(90, targetRotation.eulerAngles.y, 0);
    }
}
