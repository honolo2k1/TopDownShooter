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
    [SerializeField] private LayerMask aimLayerMask;

    [Header("Lock-On Target")]
    [SerializeField] private float lockOnRange = 30f;
    [SerializeField] private float lockOnAngle = 60f;
    [SerializeField] private LayerMask enemyLayer;

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

    // Cached references
    private Camera cachedMainCamera;
    private RaycastHit cachedFrameHit;

    // Lock-on system
    private Enemy lockedTarget;
    public bool IsLockedOn => lockedTarget != null;

    // Ground plane for fallback raycast
    private Plane groundPlane;

    private void Start()
    {
        cachedMainCamera = Camera.main;
        cameraManger = CameraManager.Instance;
        player = gameObject.GetComponent<Player>();
        groundPlane = new Plane(Vector3.up, Vector3.zero);
        AssignInputEvents();
    }

    private void Update()
    {
        if (player.Health.IsDead)
            return;

        if (player.ControlsEnable == false)
            return;

        UpdateMouseHitCache();
        UpdateLockOnTarget();
        UpdateAimVisuals();
        UpdateAimPosition();
        UpdateCameraPosition();
    }

    #region Mouse Raycast

    private void UpdateMouseHitCache()
    {
        Ray ray = cachedMainCamera.ScreenPointToRay(mouseInput);

        // Primary: raycast against aim layers
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        {
            lastKnownMouseHit = hitInfo;
            cachedFrameHit = hitInfo;
            return;
        }

        // Fallback: intersect with ground plane at player height
        groundPlane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));
        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            lastKnownMouseHit.point = hitPoint;
        }

        cachedFrameHit = lastKnownMouseHit;
    }

    public RaycastHit GetMouseHitInfo()
    {
        return cachedFrameHit;
    }

    #endregion

    #region Lock-On Target System

    private void ToggleLockOn()
    {
        if (lockedTarget != null)
        {
            // Unlock current target
            UnlockTarget();
            return;
        }

        // Find the best target to lock onto
        Enemy bestTarget = FindBestLockOnTarget();
        if (bestTarget != null)
        {
            LockOnTo(bestTarget);
        }
    }

    private void LockOnTo(Enemy enemy)
    {
        lockedTarget = enemy;
    }

    private void UnlockTarget()
    {
        lockedTarget = null;
    }

    private void UpdateLockOnTarget()
    {
        if (lockedTarget == null) return;

        // Auto-unlock if target dies, is destroyed, or is out of range
        if (lockedTarget.GetComponent<Enemy_Health>().IsDead ||
            !lockedTarget.gameObject.activeInHierarchy ||
            Vector3.Distance(transform.position, lockedTarget.transform.position) > lockOnRange * 1.5f)
        {
            // Try to find next closest enemy
            Enemy nextTarget = FindBestLockOnTarget();
            if (nextTarget != null)
                LockOnTo(nextTarget);
            else
                UnlockTarget();
        }
    }

    private Enemy FindBestLockOnTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, lockOnRange, enemyLayer);

        Enemy bestTarget = null;
        float bestScore = float.MaxValue;

        Vector3 aimDirection = (cachedFrameHit.point - transform.position).normalized;
        aimDirection.y = 0;

        foreach (Collider col in colliders)
        {
            Enemy enemy = col.GetComponentInParent<Enemy>();
            if (enemy == null) continue;
            if (enemy.GetComponent<Enemy_Health>().IsDead) continue;
            if (!enemy.gameObject.activeInHierarchy) continue;

            Vector3 dirToEnemy = (enemy.transform.position - transform.position).normalized;
            dirToEnemy.y = 0;

            float angle = Vector3.Angle(aimDirection, dirToEnemy);
            if (angle > lockOnAngle) continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            // Score: weight distance + angle (lower = better)
            float score = distance + angle * 0.5f;

            if (score < bestScore)
            {
                bestScore = score;
                bestTarget = enemy;
            }
        }

        return bestTarget;
    }

    public Transform Target()
    {
        if (lockedTarget != null)
            return lockedTarget.transform;

        return null;
    }

    #endregion

    #region Aim Position & Visuals

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

    private void UpdateAimVisuals()
    {
        aim.transform.rotation = Quaternion.LookRotation(cachedMainCamera.transform.forward);
        aimLaser.enabled = player.Weapon.WeaponReady();

        if (aimLaser.enabled == false)
            return;

        WeaponModel weaponModel = player.WeaponVisuals.CurrentWeaponModel();

        weaponModel.transform.LookAt(aim);
        weaponModel.gunPoint.LookAt(aim);

        Transform gunPoint = player.Weapon.GunPoint();
        Vector3 laserDirection = player.Weapon.BulletDirection();

        float laserTipLenght = .5f;
        float gunDistance = player.Weapon.CurrentWeapon().GunDistance;

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
            // Smooth aim to locked target (aim at center mass, slightly above ground)
            Vector3 targetPos = target.position + Vector3.up;
            aim.position = Vector3.Lerp(aim.position, targetPos, Time.deltaTime * 15f);
            return;
        }

        aim.position = cachedFrameHit.point;

        Vector3 newAimPosion = isAimingPrecisly ? aim.position : transform.position;
        aim.position = new Vector3(aim.position.x, newAimPosion.y, aim.position.z);
    }

    #endregion

    #region Camera

    private void UpdateCameraPosition()
    {
        Vector3 desired = DesieredCameraPosition();
        if (Vector3.Distance(cameraTarget.position, desired) < 0.1f)
            return;

        cameraTarget.position = Vector3.Lerp(cameraTarget.position, desired, cameraSensetivity * Time.deltaTime);
    }

    private Vector3 DesieredCameraPosition()
    {
        float actualMaxCameraDistance = player.Movement.moveInput.y < -.5f ? minCameraDistance : maxCameraDistance;

        Vector3 desiredCameraPosition = cachedFrameHit.point;
        Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;

        float distanceToDesierdPosition = Vector3.Distance(transform.position, desiredCameraPosition);
        float clampedDistance = Mathf.Clamp(distanceToDesierdPosition, minCameraDistance, actualMaxCameraDistance);

        desiredCameraPosition = transform.position + aimDirection * clampedDistance;
        desiredCameraPosition.y = transform.position.y + 1;

        return desiredCameraPosition;
    }

    #endregion

    #region Input

    private void AssignInputEvents()
    {
        controls = player.Controls;

        controls.Character.PreciseAim.performed += context => EnablePreciseAim(true);
        controls.Character.PreciseAim.canceled += context => EnablePreciseAim(false);

        controls.Character.Aim.performed += context => mouseInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => mouseInput = Vector2.zero;

        controls.Character.LockTarget.performed += context => ToggleLockOn();
    }

    #endregion

    public Transform Aim() => aim;
    public bool CanAimPrecisly() => isAimingPrecisly;
}
