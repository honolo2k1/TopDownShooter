using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using static Enums;

public class Enemy_Melee : Enemy
{
    public Enemy_MeleeSFX MeleeSFX { get; private set; }

    #region State
    public IdleState_Melee IdleState { get; private set; }
    public MoveState_Melee MoveState { get; private set; }
    public RecoveryState_Melee RecoveryState { get; private set; }
    public ChaseState_Melee ChaseState { get; private set; }
    public AttackState_Melee AttackState { get; private set; }
    public DeadState_Melee DeadState { get; private set; }
    public AbilityState_Melee AbilityState { get; private set; }
    #endregion

    [Header("Enemy Settings")]
    public EnemyMelee_Type MeleeType;
    public Enemy_MeleeWeaponType WeaponType;

    [Header("Shield")]
    public int ShieldDurability;
    public Transform ShieldTransform;

    [Header("Dodge")]
    public float DodgeCooldown;
    private float lastTimeDodge = -10;

    [Header("Axe Throw Ability")]
    public GameObject AxaPrefab;
    public float AxeFlySpeed;
    public float AxeAimTimer;
    public float AxeThrowCooldown;
    public Transform AxeStartPoint;
    public int AxeDamage;
    private float lastTimeAxeThrown;

    [Header("Attack Data")]
    public AttackData_EnemyMelee AttackData;
    public List<AttackData_EnemyMelee> AttackList;
    private Enemy_WeaponModel currentWeapon;
    private bool isAttackReady;
    [Space]
    [SerializeField] private GameObject meleeAttackFx;

    public override void Awake()
    {
        base.Awake();


        IdleState = new IdleState_Melee(this, stateMachine, "Idle");
        MoveState = new MoveState_Melee(this, stateMachine, "Move");
        RecoveryState = new RecoveryState_Melee(this, stateMachine, "Recovery");
        ChaseState = new ChaseState_Melee(this, stateMachine, "Chase");
        AttackState = new AttackState_Melee(this, stateMachine, "Attack");
        DeadState = new DeadState_Melee(this, stateMachine, "Idle"); // use ragdoll 
        AbilityState = new AbilityState_Melee(this, stateMachine, "AxeThrow");

        MeleeSFX = GetComponent<Enemy_MeleeSFX>();
    }
    public override void Start()
    {
        base.Start();

        stateMachine.Init(IdleState);

        InitPerk();

        visuals.SetupLook();
        UpdateAttackData();
    }
    public override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
        MeleeAttackCheck(currentWeapon.DamagePoints, currentWeapon.AttackRadius, meleeAttackFx, AttackData.AttackDamage);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();


        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, AttackData.AttackRange);
    }

    public override void EnterBattleMode()
    {
        if (InBattleMode) return;
        base.EnterBattleMode();
        stateMachine.ChangeState(RecoveryState);
    }
    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        WalkSpeed = WalkSpeed * 0.6f;
        visuals.EnableWeaponModel(false);
    }

    public void UpdateAttackData()
    {
        currentWeapon = visuals.CurrentWeaponModel.GetComponent<Enemy_WeaponModel>();
        if (currentWeapon != null)
        {
            if (currentWeapon.WeaponData != null)
            {
                AttackList = new List<AttackData_EnemyMelee>(currentWeapon.WeaponData.AttackData);
                TurnSpeed = currentWeapon.WeaponData.TurnSpeed;
            }
        }
    }
    protected override void InitPerk()
    {
        if (MeleeType == EnemyMelee_Type.AxeThrow)
        {
            WeaponType = Enemy_MeleeWeaponType.Throw;
        }
        if (MeleeType == EnemyMelee_Type.Shield)
        {
            anim.SetFloat("ChaseIndex", 1);
            ShieldTransform.gameObject.SetActive(true);
            WeaponType = Enemy_MeleeWeaponType.OneHand;
        }
        if (MeleeType == EnemyMelee_Type.Dodge)
        {
            WeaponType = Enemy_MeleeWeaponType.Unarmed;
        }
    }

    public override void Die()
    {
        base.Die();
        if (stateMachine.currentState != DeadState)
        {
            stateMachine.ChangeState(DeadState);
        }
    }
    public void ThrowAxe()
    {
        // Proceed if everything is valid
        GameObject newAxe = ObjectPool.Instance.GetObject(AxaPrefab, AxeStartPoint);

        newAxe.GetComponent<Enemy_Axe>().AxeSetup(AxeFlySpeed, player, AxeAimTimer, AxeDamage);
    }
    public bool CanThrowAxe()
    {
        if (MeleeType != EnemyMelee_Type.AxeThrow) { return false; }

        if (Time.time > AxeThrowCooldown + lastTimeAxeThrown)
        {
            lastTimeAxeThrown = Time.time;
            return true;
        }
        return false;

    }
    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, player.transform.position) < AttackData.AttackRange;

    public void ActiveDodgeRoll()
    {
        if (MeleeType != EnemyMelee_Type.Dodge) return;
        if (stateMachine.currentState != ChaseState) return;
        if (Vector3.Distance(transform.position, player.position) < 2f) return;

        float dodgeAnimationDuration = GetAnimationDuration("Dodge Roll");

        if (Time.time > DodgeCooldown + dodgeAnimationDuration + lastTimeDodge)
        {
            lastTimeDodge = Time.time;
            anim.SetTrigger("Dodge");

            ragdoll.CollidersActive(false);
            EnableCollidersAfterDelay(0.5f).Forget(); // Start the task with a 0.5s delay
        }
    }

    // Task to enable colliders after a delay
    private async UniTaskVoid EnableCollidersAfterDelay(float delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: this.GetCancellationTokenOnDestroy()); // Wait for 0.5 seconds
        ragdoll.CollidersActive(true); // Enable the colliders
    }

    private float GetAnimationDuration(string clipName)
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }

        Debug.Log(clipName + "animation not found");

        return 0;
    }




}
