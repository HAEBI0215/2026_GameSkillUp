using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator anim;

    [Header("State")]
    public EnemyState currentState = EnemyState.Idle;

    [Header("Vision")]
    public float viewDistance = 10f;
    public float viewAngle = 90f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;
    public Enemy enemy;

    [Header("Combat")]
    public float attackRange = 2f;
    public float chaseRange = 12f;
    public float attackCooldown = 1.5f;

    [Header("Lose Target")]
    public float loseTargetTime = 10f;
    private float lastSeenTime;

    private float lastAttackTime;
    private bool isDead = false;
    private float alertTimer;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (anim == null) anim = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();

        ChangeState(EnemyState.Idle);
    }

    void Update()
    {
        if (isDead) return;

        switch (currentState)
        {
            case EnemyState.Idle:
                UpdateIdle();
                break;

            case EnemyState.Alert:
                UpdateAlert();
                break;

            case EnemyState.Chase:
                UpdateChase();
                break;

            case EnemyState.Attack:
                UpdateAttack();
                break;

            case EnemyState.Hit:
                break;
        }
    }


    void UpdateIdle()
    {
        agent.isStopped = true;
        alertTimer += Time.deltaTime;

        if (alertTimer >= 3f)
        {
            alertTimer = 0f;
            ChangeState(EnemyState.Idle);
        }

        if (enemy.isAware)
        {
            ChangeState(EnemyState.Chase);
        }
    }

    void UpdateAlert()
    {
        anim.SetBool("isRun", false);
        agent.isStopped = true;

        alertTimer += Time.deltaTime;

        if (alertTimer >= 3f)
        {
            alertTimer = 0f;
            ChangeState(EnemyState.Idle);
        }
    }

    void UpdateChase()
    {
        if (enemy.isAware)
        {
            lastSeenTime = Time.time;
            anim.SetBool("isRun", true);
        }

        if (Time.time - lastSeenTime > loseTargetTime)
        {
            ChangeState(EnemyState.Alert);
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(player.position);
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= attackRange)
        {
            ChangeState(EnemyState.Attack);
        }
    }

    void UpdateAttack()
    {
        if (player == null) return;

        agent.isStopped = true;
        anim.SetBool("isRun", false);

        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0f;

        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
        }

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > attackRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            anim.SetTrigger("Attack");
        }
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > viewDistance) return false;

        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > viewAngle * 0.5f) return false;

        if (Physics.Raycast(transform.position + Vector3.up, dirToPlayer, dist, obstacleLayer))
        {
            return false;
        }

        if (Physics.Raycast(transform.position + Vector3.up, dirToPlayer, dist, playerLayer))
        {
            return true;
        }

        return false;
    }


    void ChangeState(EnemyState newState)
    {
        currentState = newState;

        if (newState == EnemyState.Idle)
        {
            agent.isStopped = true;
        }
    }


    public void TakeDamage()
    {
        if (isDead) return;

        anim.SetTrigger("Hit");
        ChangeState(EnemyState.Hit);

        Invoke(nameof(GoChase), 0.2f);
    }

    void GoChase()
    {
        if (isDead) return;
        ChangeState(EnemyState.Chase);
    }

    public void Die()
    {
        isDead = true;
        agent.isStopped = true;
        anim.SetTrigger("Die");
        ChangeState(EnemyState.Dead);
    }
    public void FootR()
    {

    }
    public void FootL()
    {

    }
}
