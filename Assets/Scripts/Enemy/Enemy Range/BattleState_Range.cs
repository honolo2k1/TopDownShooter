using UnityEngine;
using static Enums;

public class BattleState_Range : EnemyState
{
    public Enemy_Range Enemy;
    private float lastTimeShot = -10f;
    private int bulletsShot = 0;
    private int bulletsPerAttack;
    private float weaponCooldown;

    private float coverCheckTimer;
    private bool firstTimeAttack = true;
    public BattleState_Range(Enemy enemyBase, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemyBase, enemyStateMachine, animBoolName)
    {
        Enemy = enemyBase as Enemy_Range;
    }
    public override void Enter()
    {
        base.Enter();

        SetupValueForFirstAttack();

        Enemy.agent.isStopped = true;
        Enemy.agent.velocity = Vector3.zero;

        Enemy.visuals.EnableIK(true, true);

        stateTimer = Enemy.AttackDelay;
    }

    public override void Update()
    {
        base.Update();

        if (Enemy.IsSeeingPlayer())
            Enemy.FaceTarget(Enemy.Aim.position);

        if (Enemy.CanThrowGrenade())
        {
            stateMachine.ChangeState(Enemy.ThrowGrenadeState);
            return;
        }
        if (MustAdvancePlayer())
            stateMachine.ChangeState(Enemy.AdvancePlayerState);

        ChangeCoverIfShould();

        if (stateTimer > 0)
            return;

        if (WeaponOutOfBullets())
        {
            if (Enemy.IsUnstoppable() && UnStoppableWalkReady())
            {
                Enemy.AdvanceDuration = weaponCooldown;
                stateMachine.ChangeState(Enemy.AdvancePlayerState);
            }
            if (WeaponOnCooldown())
            {
                AttemptToResetWeapon();
            }
            return;
        }

        if (CanShoot() && Enemy.IsAimOnPlayer())
            Shoot();
    }

    private bool MustAdvancePlayer()
    {
        if (Enemy.IsUnstoppable())
        {
            return false;
        }

        return !Enemy.IsPlayerInAgressionRange() && ReadyToLeaveCover();
    }

    private bool UnStoppableWalkReady()
    {
        float distanceToPlayer = Vector3.Distance(Enemy.transform.position, Enemy.player.position);
        bool outOfStoppingDistance = distanceToPlayer > Enemy.AdvanceStoppingDistance;

        bool unStoppableWalkOnCooldown = Time.time < Enemy.WeaponData.MinWeaponCooldown + Enemy.AdvancePlayerState.LastTimeAdvance;

        return outOfStoppingDistance && unStoppableWalkOnCooldown == false;
    }

    #region Cover System Region
    private void ChangeCoverIfShould()
    {
        if (Enemy.CoverPerk != CoverPerk.CanTakeAndChangeCover)
        {
            return;
        }

        coverCheckTimer -= Time.deltaTime;

        if (coverCheckTimer < 0)
        {
            coverCheckTimer = 1f;
            if (ReadyToChangeCover() && ReadyToLeaveCover())
            {
                if (Enemy.CanGetCover())
                {
                    stateMachine.ChangeState(Enemy.RunToCoverState);
                }
            }
        }
    }
    private bool ReadyToLeaveCover()
    {
        return Time.time > Enemy.MinCoverTime + Enemy.RunToCoverState.LastTimeTookCover;
    }
    private bool ReadyToChangeCover()
    {
        bool inDanger = IsPlayerInClearSight() && IsPlayerClose();
        bool advanceTimeIsOver = Time.time > Enemy.AdvancePlayerState.LastTimeAdvance + Enemy.AdvanceDuration;

        return inDanger && advanceTimeIsOver;
    }
    private bool IsPlayerClose()
    {
        return Vector3.Distance(Enemy.transform.position, Enemy.player.transform.position) < Enemy.SafeDistance;
    }
    private bool IsPlayerInClearSight()
    {
        Vector3 directionToPlayer = Enemy.player.transform.position - Enemy.transform.position;

        if (Physics.Raycast(Enemy.transform.position, directionToPlayer, out RaycastHit hit))
        {
            {
                if (/*hit.transform.parent == Enemy.player ||*/ hit.transform.root == Enemy.player.root /*|| hit.transform.parent.parent == Enemy.player*/)
                {
                    return true;
                }
            }
        }
        return false;
    }
#endregion

    #region Weapon region
    private void AttemptToResetWeapon()
    {
        bulletsShot = 0;
        bulletsPerAttack = Enemy.WeaponData.GetBulletsPerAttack();
        weaponCooldown = Enemy.WeaponData.GetWeaponCooldown();
    }
    private bool WeaponOnCooldown() => Time.time > lastTimeShot + weaponCooldown;
    private bool WeaponOutOfBullets() => bulletsShot >= bulletsPerAttack;
    private bool CanShoot() => Time.time > lastTimeShot + 1 / Enemy.WeaponData.FireRate;
    private void Shoot()
    {
        Enemy.FireSingleBullet();
        lastTimeShot = Time.time;
        bulletsShot++;
    }
    private void SetupValueForFirstAttack()
    {
        if (firstTimeAttack)
        {
            Enemy.AggressionRange = Enemy.AdvanceStoppingDistance + 2;

            firstTimeAttack = false;
            bulletsPerAttack = Enemy.WeaponData.GetBulletsPerAttack();
            weaponCooldown = Enemy.WeaponData.GetWeaponCooldown();
        }
    }
    #endregion
}
