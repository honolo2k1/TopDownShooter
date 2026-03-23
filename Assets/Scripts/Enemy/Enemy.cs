using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System;
using TMPro;
using static Enums;


[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public EnemyType EnemyType;
    public LayerMask WhatIsAlly;
    public LayerMask WhatIsPlayer;

    [SerializeField] private GameObject miniMapDot;
    [Space]
    [Header("Idle Date")]
    public float IdleTime;
    public float AggressionRange;
    [Header("Move Date")]
    public float TurnSpeed;
    public float WalkSpeed = 1.5f;
    public float RunSpeed = 3f;

    private bool manualMovement;
    private bool manualRotation;

    [SerializeField] private Transform[] patrolPoints;
    private Vector3[] patrolPointsPosition;
    private int currentPatrolIndex;
    protected bool isMeleeAttackReady;

    public bool InBattleMode { get; private set; }
    public Enemy_Visuals visuals { get; private set; }
    public Transform player { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public EnemyStateMachine stateMachine { get; private set; }
    public Animator anim { get; private set; }
    public Ragdoll ragdoll { get; private set; }
    public Enemy_Health health { get; private set; }

    public Enemy_DropController dropController { get; private set; }

    public AudioManager audioManager { get; private set; }

    public virtual void Awake()
    {
        dropController = GetComponent<Enemy_DropController>();
        health = GetComponent<Enemy_Health>();
        ragdoll = GetComponent<Ragdoll>();
        visuals = GetComponent<Enemy_Visuals>();
        stateMachine = new EnemyStateMachine();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        player = GameManager.Instance.Player.transform;
    }
    public virtual void Start()
    {
        InitPatrolPoints();

        audioManager = AudioManager.Instance;
    }
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, AggressionRange);
    }
    public virtual void Update()
    {
        if (ShouldEnterBattleMode())
        {
            EnterBattleMode();
        }
    }
    protected virtual void InitPerk()
    {

    }
    public virtual void MakeEnemyVIP()
    {
        int additionalHeath = Mathf.RoundToInt(health.CurrentHealth * 1.5f);
        health.CurrentHealth += additionalHeath;
        transform.localScale = transform.localScale * 1.15f;
    }
    protected bool ShouldEnterBattleMode()
    {
        if (IsPlayerInAgressionRange() && !InBattleMode)
        {
            EnterBattleMode();
            return true;
        }
        return false;
    }

    public virtual void EnterBattleMode()
    {
        InBattleMode = true;
    }

    public virtual void GetHit(int damage)
    {
        if (health.IsDead)
        {
            return;
        }

        EnterBattleMode();
        health.ReduceHealth(damage);

        health.UpdateHeathUI(health.CurrentHealth, health.MaxHealth);
        if (health.FloatingTextPrefab != null && !health.IsDead)
        {
            ShowFloatingText(damage);
        }

        if (health.ShouldDie())
        {
            Die();
        }
    }

    private void ShowFloatingText(int damage)
    {
        var floatingText = ObjectPool.Instance.GetObject(health.FloatingTextPrefab, transform.position);
        floatingText.GetComponent<TextMeshPro>().text = $"-{damage}";
    }

    public virtual void Die()
    {
        if (health.IsDead)
            return;

        health.IsDead = true;

        dropController.DropItem();

        miniMapDot.SetActive(false);

        anim.enabled = false;

        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
        agent.enabled = false;

        ragdoll.RagdollActive(false);

        health.HealthBar.SetActive(false);

        MissionObject_HuntTarget huntTarget = GetComponent<MissionObject_HuntTarget>();
        huntTarget?.InvokeOnTargetKilled();
    }

    public virtual void MeleeAttackCheck(Transform[] damagePoints, float attackCheckRadius, GameObject fx, int damage)
    {
        if (!isMeleeAttackReady)
        {
            return;
        }
        foreach (Transform attackPoint in damagePoints)
        {
            Collider[] detectedHits = Physics.OverlapSphere(attackPoint.position, attackCheckRadius, WhatIsPlayer);

            for (int i = 0; i < detectedHits.Length; i++)
            {
                IDamagable damagable = detectedHits[i].GetComponent<IDamagable>();
                if (damagable != null)
                {
                    damagable.TakeDamage(damage);
                    isMeleeAttackReady = false;

                    GameObject newAttackFx = ObjectPool.Instance.GetObject(fx, attackPoint);
                    ObjectPool.Instance.ReturnObject(newAttackFx, 1);
                    return;
                }
            }
        }
    }
    public void EnableMeleeAttackCheck(bool enable) => isMeleeAttackReady = enable;
    public virtual void BulletImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        if (health.ShouldDie())
        {
            DeadImpactCoroutine(force, hitPoint, rb).Forget();
        }
    }
    private async UniTaskVoid DeadImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: this.GetCancellationTokenOnDestroy());

        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    public void FaceTarget(Vector3 target, float turnSpeed = 0)
    {
        Vector3 direction = target - transform.position;

        if (direction.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        Vector3 currentEulerAngles = transform.rotation.eulerAngles;

        if (turnSpeed == 0)
        {
            turnSpeed = this.TurnSpeed;
        }

        float yRotation = Mathf.LerpAngle(currentEulerAngles.y, targetRotation.eulerAngles.y, TurnSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(currentEulerAngles.x, yRotation, currentEulerAngles.z);
    }

    #region Animation Events
    public void ActiveManualMovement(bool manualMovement) => this.manualMovement = manualMovement;
    public bool ManualMovementActive() => manualMovement;

    public void ActiveManualRotation(bool manualRotation) => this.manualRotation = manualRotation;
    public bool ManualRotationActive() => manualRotation;
    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();

    public virtual void AbilityTrigger()
    {
        stateMachine.currentState.AbilityTrigger();
    }
    #endregion

    #region Patrols Logic
    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPointsPosition[currentPatrolIndex];

        currentPatrolIndex++;
        if (currentPatrolIndex >= patrolPoints.Length)
        {
            currentPatrolIndex = 0;
        }

        return destination;
    }
    private void InitPatrolPoints()
    {
        patrolPointsPosition = new Vector3[patrolPoints.Length];

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPointsPosition[i] = patrolPoints[i].position;
            patrolPoints[i].gameObject.SetActive(false);
        }
    }
    #endregion
    public bool IsPlayerInAgressionRange() => Vector3.Distance(transform.position, player.transform.position) < AggressionRange;
}
