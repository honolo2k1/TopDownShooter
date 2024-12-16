using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class Enemy_Boss : Enemy
{
    public Enemy_BossSFX BossSFX { get; private set; }

    [Header("Boss Details")]
    public BossWeaponType BossWeaponType;
    public float ActionCooldown = 10;
    public float AttackRange;

    [Header("Ability")]
    public float MinAbilityDistance;
    public float AbilityCooldown;
    private float lastTimeUseAbility;
    [Header("Flamethrower")]
    public int FlameDamage;
    public float FlameDamageCooldown;
    public ParticleSystem Flamethrower;
    public float FlamethrowDuration;
    public bool FlamethrowActive { get; private set; }
    [Header("Fist")]
    public int FistDamage;
    public GameObject ActivationPrefab;
    [SerializeField] private float FistCheckRadius;
    [Header("Jump Attack")]
    public int JumpAttackDamage;
    public float JumpAttackCooldown = 10;
    private float lastTimeJumped;
    public float TravelTimeToTarget = 1f;
    public float MinJumpDistanceRequired;
    [Space]
    [SerializeField] private float upwardsMultiplier = 5f;
    public float ImpactRadius = 2.5f;
    public float ImpactPower = 7;
    public Transform ImpactPoint;
    [Space]
    [SerializeField] private LayerMask whatToIngore;
    [Header("Attack")]
    [SerializeField] private int meleeAttackDamage;
    [SerializeField] private Transform[] damagePoints;
    [SerializeField] private float attackCheckRadius;
    [SerializeField] private GameObject meleeAttackFx;

    public IdleState_Boss IdleState { get; private set; }
    public MoveState_Boss MoveState { get; private set; }
    public AttackState_Boss AttackState { get; private set; }
    public JumpAttackState_Boss JumpAttackState { get; private set; }
    public AbilityState_Boss AbilityState { get; private set; }
    public DeadState_Boss DeadState { get; private set; }
    public Enemy_BossVisuals BossVisuals { get; private set; }

    public override void Awake()
    {
        base.Awake();

        BossVisuals = GetComponent<Enemy_BossVisuals>();

        IdleState = new IdleState_Boss(this, stateMachine, "Idle");
        MoveState = new MoveState_Boss(this, stateMachine, "Move");
        AttackState = new AttackState_Boss(this, stateMachine, "Attack");
        JumpAttackState = new JumpAttackState_Boss(this, stateMachine, "JumpAttack");
        AbilityState = new AbilityState_Boss(this, stateMachine, "Ability");

        DeadState = new DeadState_Boss(this, stateMachine, "Idle");


        BossSFX = GetComponent<Enemy_BossSFX>();
    }

    public override void Start()
    {
        base.Start();

        stateMachine.Init(IdleState);
    }

    public override void Update()
    {
        base.Update();
        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    stateMachine.ChangeState(AbilityState);
        //}
        stateMachine.currentState.Update();

        if (ShouldEnterBattleMode())
        {
            EnterBattleMode();
        }

        MeleeAttackCheck(damagePoints, attackCheckRadius, meleeAttackFx, meleeAttackDamage);
    }

    public override void Die()
    {
        base.Die();

        if (stateMachine.currentState != DeadState)
        {
            stateMachine.ChangeState(DeadState);
        }
    }
    public override void EnterBattleMode()
    {
        if (InBattleMode) return;
        base.EnterBattleMode();

        stateMachine.ChangeState(MoveState);
    }
    public void ActivateFlamethrower(bool active)
    {
        FlamethrowActive = active;
        if (!active)
        {
            Flamethrower.Stop();
            anim.SetTrigger("StopFlamethrower");
            return;
        }
        var mainModule = Flamethrower.main;
        var extraModule = Flamethrower.transform.GetChild(0).GetComponent<ParticleSystem>().main;

        mainModule.duration = FlamethrowDuration;
        extraModule.duration = FlamethrowDuration;

        Flamethrower.Clear();
        Flamethrower.Play();
    }
    public void ActivateFirt()
    {
        GameObject newActivation = ObjectPool.Instance.GetObject(ActivationPrefab, ImpactPoint);
        ObjectPool.Instance.ReturnObject(newActivation, 1);

        MassDamage(damagePoints[1].position, FistCheckRadius, FistDamage);
    }
    public bool CanDoAbility()
    {
        bool playerWithinDistance = Vector3.Distance(transform.position, player.position) < MinAbilityDistance;

        if (!playerWithinDistance)
            return false;

        if (Time.time > lastTimeUseAbility + AbilityCooldown)
        {
            return true;
        }
        return false;
    }
    public void SetAbilityOnCooldown() => lastTimeUseAbility = Time.time;
    public void JumpImpact()
    {
        Transform impactPoint = this.ImpactPoint;
        if (impactPoint == null)
        {
            impactPoint = transform;
        }
        MassDamage(impactPoint.position, ImpactRadius, JumpAttackDamage);
    }

    private void MassDamage(Vector3 impactPoint, float impactRadius, int damage)
    {
        HashSet<GameObject> uniqueEntities = new HashSet<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(impactPoint, impactRadius, ~WhatIsAlly);
        foreach (Collider hit in colliders)
        {
            IDamagable damagable = hit.GetComponent<IDamagable>();
            if (damagable != null)
            {
                GameObject rootEntity = hit.transform.root.gameObject;
                if (!uniqueEntities.Add(rootEntity))
                    continue;
                damagable.TakeDamage(damage);
            }

            ApplyPhysicalForceTo(impactPoint, impactRadius, hit);

        }
    }

    private void ApplyPhysicalForceTo(Vector3 impactPoint, float impactRadius, Collider hit)
    {
        Rigidbody rb = hit.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddExplosionForce(ImpactPower, impactPoint, impactRadius, upwardsMultiplier, ForceMode.Impulse);
        }
    }

    public bool CanDoJumpAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < MinJumpDistanceRequired)
        {
            return false;
        }
        if (Time.time > lastTimeJumped + JumpAttackCooldown && IsPlayerInClearSight())
        {
            return true;
        }
        return false;
    }
    public void SetJumpAttackOnCooldown() => lastTimeJumped = Time.time;
    public bool IsPlayerInClearSight()
    {
        Vector3 myPos = transform.position + new Vector3(0, 1.6f, 0);
        Vector3 playerPos = player.position + Vector3.up;
        Vector3 directionToPlayer = (playerPos - myPos).normalized;
        if (Physics.Raycast(myPos, directionToPlayer, out RaycastHit hit, 100, ~whatToIngore))
        {
            if (hit.transform.root == player.root/* || hit.transform.parent.parent == player || hit.transform.parent == player*/)
            {
                return true;
            }
        }
        return false;
    }

    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, player.transform.position) < AttackRange;
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(transform.position, AttackRange);
        if (player != null)
        {
            Vector3 myPos = transform.position + new Vector3(0, 1.6f, 0);
            Vector3 playerPos = player.position + Vector3.up;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(myPos, playerPos);
        }
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, MinAbilityDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ImpactRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, MinJumpDistanceRequired);

        if (damagePoints.Length > 0)
        {
            foreach (var damagePoint in damagePoints)
            {
                Gizmos.DrawWireSphere(damagePoint.position, attackCheckRadius);
            }

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(damagePoints[1].position, FistCheckRadius);
        }
    }
}
