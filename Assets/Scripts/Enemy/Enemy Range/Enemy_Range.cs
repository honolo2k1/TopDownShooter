using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class Enemy_Range : Enemy
{
    [Header("Enemy Perks")]
    public Enemy_RangeWeaponType WeaponType;
    public CoverPerk CoverPerk;
    public UnstoppablePerk UnstoppablePerk;
    public GrenadePerk GrenadePerk;

    [Header("Grenade Perk")]
    public GameObject GrenadePrefab;
    public float ImpactPower;
    public float ExplosionTimer = 0.75f;
    public float TimeToTarget = 1.2f;
    public float GrenadeCooldown;
    public int GrenadeDamage;
    private float lastTimeGrenadeThrown = -10;
    [SerializeField] private Transform grenadeStartPoint;

    [Header("Advance Perk")]
    public float AdvanceSpeed;
    public float AdvanceStoppingDistance;
    public float AdvanceDuration = 2.5f;

    [Header("Cover System")]
    public float MinCoverTime;
    public float SafeDistance;
    public CoverPoint LastCover { get; private set; }
    public CoverPoint CurrentCover { get; private set; }

    [Header("Weapon Details")]
    public float AttackDelay;
    public Enemy_RangeWeaponData WeaponData;

    [Space]
    public Transform WeaponHolder;
    public Transform GunPoint;
    public GameObject BulletPrefab;

    [Header("Aim Details")]
    public float SlowAim = 4;
    public float FastAim = 20;
    public Transform Aim;
    public Transform PlayerBody;
    public LayerMask WhatToIgnore;

    [SerializeField] private List<Enemy_RangeWeaponData> availibleWeaponData;

    #region States
    public IdlState_Range IdleState { get; private set; }
    public MoveState_Range MoveState { get; private set; }
    public BattleState_Range BattleState { get; private set; }
    public RunToCoverState_Range RunToCoverState { get; private set; }
    public AdvancePlayerState_Range AdvancePlayerState { get; private set; }
    public ThrowGrenadeState_Range ThrowGrenadeState { get; private set; }
    public DeadState_Range DeadState { get; private set; }
    #endregion

    public override void Awake()
    {
        base.Awake();

        IdleState = new IdlState_Range(this, stateMachine, "Idle");
        MoveState = new MoveState_Range(this, stateMachine, "Move");
        BattleState = new BattleState_Range(this, stateMachine, "Battle");
        RunToCoverState = new RunToCoverState_Range(this, stateMachine, "Run");
        AdvancePlayerState = new AdvancePlayerState_Range(this, stateMachine, "Advance");
        ThrowGrenadeState = new ThrowGrenadeState_Range(this, stateMachine, "ThrowGrenade");
        DeadState = new DeadState_Range(this, stateMachine, "Idle");
    }

    public override void Start()
    {
        base.Start();

        PlayerBody = player.GetComponent<Player>().playerBody;
        Aim.parent = null;

        InitPerk();

        stateMachine.Init(IdleState);
        visuals.SetupLook();
        SetupWeapon();
    }

    public override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
    }
    public override void Die()
    {
        base.Die();

        if (stateMachine.currentState != DeadState)
        {
            stateMachine.ChangeState(DeadState);
        }
    }
    public bool CanThrowGrenade()
    {
        if (GrenadePerk == GrenadePerk.Unavalible)
            return false;

        if (Vector3.Distance(player.transform.position, transform.position) < SafeDistance)
            return false;

        if (Time.time > GrenadeCooldown + lastTimeGrenadeThrown)
            return true;

        return false;
    }
    public void ThrowGrenade()
    {
        lastTimeGrenadeThrown = Time.time;
        visuals.EnableGrenadeModel(false);

        GameObject newGrenade = ObjectPool.Instance.GetObject(GrenadePrefab, grenadeStartPoint);

        Enemy_Grenade newGrenadeScript = newGrenade.GetComponent<Enemy_Grenade>();

        if (stateMachine.currentState == DeadState)
        {
            newGrenadeScript.SetupGrenade(WhatIsAlly, transform.position, 1, ExplosionTimer, ImpactPower, GrenadeDamage);
            return;
        }

        newGrenadeScript.SetupGrenade(WhatIsAlly, player.transform.position, TimeToTarget, ExplosionTimer, ImpactPower, GrenadeDamage);
    }

    protected override void InitPerk()
    {
        if (WeaponType == Enemy_RangeWeaponType.Random)
        {
            ChooseRandomWeaponType();
        }
        if (IsUnstoppable())
        {
            AdvanceSpeed = 1;
            anim.SetFloat("AdvanceAnimIndex", 1);
        }
    }

    private void ChooseRandomWeaponType()
    {
        List<Enemy_RangeWeaponType> validTypes = new List<Enemy_RangeWeaponType>();
        foreach (Enemy_RangeWeaponType value in Enum.GetValues(typeof(Enemy_RangeWeaponType)))
        {
            if (value != Enemy_RangeWeaponType.Random && value != Enemy_RangeWeaponType.SniperRifle)
            {
                validTypes.Add(value);
            }
        }

        int randomIndex = UnityEngine.Random.Range(0, validTypes.Count);
        WeaponType = validTypes[randomIndex];
    }

    public override void EnterBattleMode()
    {
        if (InBattleMode)
            return;
        base.EnterBattleMode();
        if (CanGetCover())
        {
            stateMachine.ChangeState(RunToCoverState);
        }
        else
        {
            stateMachine.ChangeState(BattleState);
        }
    }

    #region Cover System
    public bool CanGetCover()
    {
        if (CoverPerk == CoverPerk.Unavalible)
        {
            return false;
        }

        CurrentCover = AttempToFindCover()?.GetComponent<CoverPoint>();

        if (LastCover != CurrentCover && CurrentCover != null)
        {
            return true;
        }
        return false;
    }

    private Transform AttempToFindCover()
    {
        List<CoverPoint> collectedCoverPoints = new List<CoverPoint>();
        foreach (Cover cover in CollectNearByCovers())
        {
            collectedCoverPoints.AddRange(cover.GetValidCoverPoints(transform));
        }

        CoverPoint closetCoverPoint = null;
        float shortestDistance = float.MaxValue;

        foreach (CoverPoint coverPoint in collectedCoverPoints)
        {
            float currentDistance = Vector3.Distance(transform.position, coverPoint.transform.position);

            if (currentDistance < shortestDistance)
            {
                closetCoverPoint = coverPoint;
                shortestDistance = currentDistance;
            }
        }
        if (closetCoverPoint != null)
        {
            LastCover?.SetOccupied(false);
            LastCover = CurrentCover;

            CurrentCover = closetCoverPoint;
            CurrentCover.SetOccupied(true);

            return CurrentCover.transform;
        }

        return null;
    }
    private List<Cover> CollectNearByCovers()
    {
        float coverRadiusCheck = 30;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, coverRadiusCheck);
        List<Cover> collectedCovers = new List<Cover>();

        foreach (Collider collider in hitColliders)
        {
            Cover cover = collider.GetComponent<Cover>();
            if (cover != null && collectedCovers.Contains(cover) == false)
            {
                collectedCovers.Add(cover);
            }
        }

        return collectedCovers;
    }
    #endregion

    public void FireSingleBullet()
    {
        anim.SetTrigger("Shoot");

        Vector3 bulletDirection = (Aim.position - GunPoint.position).normalized;

        GameObject newBullet = ObjectPool.Instance.GetObject(BulletPrefab, GunPoint);

        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint.forward);

        newBullet.GetComponent<Bullet>().BulletSetup(WhatIsAlly, WeaponData.BulletDamage);

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Vector3 bulletDirectionWithSpread = WeaponData.ApplyWeaponSpread(bulletDirection);

        rbNewBullet.mass = 20 / WeaponData.BulletSpeed;
        rbNewBullet.linearVelocity = bulletDirectionWithSpread * WeaponData.BulletSpeed;


    }
    private void SetupWeapon()
    {
        List<Enemy_RangeWeaponData> filteredData = new List<Enemy_RangeWeaponData>();
        foreach (var weaponData in availibleWeaponData)
        {
            if (weaponData.WeaponType == WeaponType)
                filteredData.Add(weaponData);
        }

        if (filteredData.Count > 0)
        {
            int random = UnityEngine.Random.Range(0, filteredData.Count);
            WeaponData = filteredData[random];
        }
        else
        {
            Debug.LogWarning("No available weapon data was found for " + WeaponType + "!");
        }

        GunPoint = visuals.CurrentWeaponModel.GetComponent<Enemy_RangeWeaponModel>().Gunpoint;
    }

    #region Enemy Aim Region
    public void UpdateAimPosition()
    {
        float aimSpeed = IsAimOnPlayer() ? FastAim : SlowAim;
        Aim.position = Vector3.MoveTowards(Aim.position, PlayerBody.position, aimSpeed);
    }

    public bool IsAimOnPlayer()
    {
        float distanceAimToPlayer = Vector3.Distance(Aim.position, player.position);

        return distanceAimToPlayer < 1;
    }
    public bool IsSeeingPlayer()
    {
        Vector3 enemyPosition = transform.position + Vector3.up;
        Vector3 directionToPlayer = PlayerBody.position - enemyPosition;

        if (Physics.Raycast(enemyPosition, directionToPlayer, out RaycastHit hit, Mathf.Infinity, ~WhatToIgnore))
        {
            //Debug.Log(hit.transform.name);
            if (hit.transform.root == player.root)
            {
                UpdateAimPosition();
                return true;
            }
        }
        return false;
    }
    #endregion

    public bool IsUnstoppable() => UnstoppablePerk == UnstoppablePerk.Unstoppable;

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, AdvanceStoppingDistance);
    }
}
