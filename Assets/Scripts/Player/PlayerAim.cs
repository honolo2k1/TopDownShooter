using System;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private CameraManager cameraManger;
    private Player player;
    private PlayerControls controls;
    [Header("Aim Visual - Laser")]
    [SerializeField] private LineRenderer aimLaser;

    [Header("Aim Control")]
    [SerializeField] private float preciseAimCamDistance = 6;
    [SerializeField] private float regularAimCamDistance = 7;
    [SerializeField] private float camChangeRate = 5;
    [Space]

    [Header("Aim Setup")]
    public Transform aim;
    [SerializeField] private bool isAimingPrecisly;
    [SerializeField] private float offsetChangeRate = 6;
    private float offsetY;

    [Header("Aim Layers")]
    [SerializeField] private LayerMask preciseAim;
    [SerializeField] private LayerMask regularAim;

    [Header("Camera Control")]
    [SerializeField] private Transform cameraTarget;
    [Range(0.5f, 1f)]
    [SerializeField] private float minCameraDistance = 1.5f;
    [Range(1f, 3f)]
    [SerializeField] private float maxCameraDistance = 4f;
    [Range(3f, 5f)]
    [SerializeField] private float cameraSensetivity = 5f;

    private Vector2 mouseInput;
    private RaycastHit lastKnownMouseHit;

    private void Start()
    {
        cameraManger = CameraManager.Instance;
        player = gameObject.GetComponent<Player>();
        AssignInputEvents();

    }
    private void Update()
    {
        if (player.health.IsDead)
            return;

        if (player.controlsEnable == false)
            return;

        UpdateAimVisuals();
        UpdateAimPosition();
        UpdateCameraPosition();
    }

    private void EnablePreciseAim(bool enable)
    {
        isAimingPrecisly = !isAimingPrecisly;

        Cursor.visible = false;

        if (enable)
        {
            cameraManger.ChangeCameraDistance(preciseAimCamDistance, camChangeRate);
            Time.timeScale = 0.9f;
        }
        else
        {
            cameraManger.ChangeCameraDistance(regularAimCamDistance, camChangeRate);
            Time.timeScale = 1f;
        }
    }
    public Transform GetAimCameraTarget()
    {
        cameraTarget.position = player.transform.position;
        return cameraTarget;
    }
    public void EnableAimLaser(bool enable) => aimLaser.enabled = enable;


    private void AssignInputEvents()
    {
        controls = player.controls;

        controls.Character.PreciseAim.performed += context => EnablePreciseAim(true);
        controls.Character.PreciseAim.canceled += context => EnablePreciseAim(false);

        controls.Character.Aim.performed += context => mouseInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => mouseInput = Vector2.zero;
    }


    private void UpdateAimVisuals()
    {
        aim.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        aimLaser.enabled = player.weapon.WeaponReady();

        if (aimLaser.enabled == false)
            return;


        WeaponModel weaponModel = player.weaponVisuals.CurrentWeaponModel();

        weaponModel.transform.LookAt(aim);
        weaponModel.gunPoint.LookAt(aim);


        Transform gunPoint = player.weapon.GunPoint();
        Vector3 laserDirection = player.weapon.BulletDirection();

        float laserTipLenght = .5f;
        float gunDistance = player.weapon.CurrentWeapon().GunDistance;

        Vector3 endPoint = gunPoint.position + laserDirection * gunDistance;

        if (Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hit, gunDistance))
        {
            endPoint = hit.point;
            laserTipLenght = 0;
        }

        aimLaser.SetPosition(0, gunPoint.position);
        aimLaser.SetPosition(1, endPoint);
        aimLaser.SetPosition(2, endPoint + laserDirection * laserTipLenght);
    }

    private void UpdateAimPosition()
    {
        Transform target = Target();

        if (target != null)
        {
            aim.position = target.position;
            return;
        }


        aim.position = GetMouseHitInfo().point;

        Vector3 newAimPosion = isAimingPrecisly ? aim.position : transform.position;

        aim.position = new Vector3(aim.position.x, newAimPosion.y/* + AjustOffsetY()*/, aim.position.z);
    }

    private float AjustOffsetY()
    {
        if (isAimingPrecisly)
        {
            offsetY = Mathf.Lerp(offsetY, 0, Time.deltaTime * offsetChangeRate * 0.5f);
        }
        else
        {
            offsetY = Mathf.Lerp(offsetY, 1, Time.deltaTime * offsetChangeRate);
        }
        return offsetY;
    }

    #region Camera Region
    private void UpdateCameraPosition()
    {
        bool camMoveCamera = Vector3.Distance(cameraTarget.position, DesieredCameraPosition()) > 1;

        if (!camMoveCamera)
            return;

        cameraTarget.position =
                    Vector3.Lerp(cameraTarget.position, DesieredCameraPosition(), cameraSensetivity * Time.deltaTime);
    }

    private Vector3 DesieredCameraPosition()
    {
        float actualMaxCameraDistance = player.movement.moveInput.y < -.5f ? minCameraDistance : maxCameraDistance;

        Vector3 desiredCameraPosition = GetMouseHitInfo().point;
        Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;

        float distanceToDesierdPosition = Vector3.Distance(transform.position, desiredCameraPosition);
        float clampedDistance = Mathf.Clamp(distanceToDesierdPosition, minCameraDistance, actualMaxCameraDistance);

        desiredCameraPosition = transform.position + aimDirection * clampedDistance;
        desiredCameraPosition.y = transform.position.y + 1;

        return desiredCameraPosition;
    }


    #endregion

    public RaycastHit GetMouseHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseInput);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, preciseAim))
        {
            lastKnownMouseHit = hitInfo;
            return hitInfo;
        }

        return lastKnownMouseHit;
    }
    public Transform Target()
    {
        Transform target = null;

        Debug.LogError(GetMouseHitInfo().transform.gameObject.name);
        if (GetMouseHitInfo().transform.gameObject.GetComponentInParent<Enemy>() != null)
        {
            target = GetMouseHitInfo().transform;
        }

        return target;
    }


    public Transform Aim() => aim;
    public bool CanAimPrecisly() => isAimingPrecisly;

}
