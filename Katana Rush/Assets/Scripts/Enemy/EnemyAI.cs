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
    public EnemyHitbox attackHitbox;
    public float hitboxActiveDuration = 0.5f; // 히트박스가 활성화될 시간

    [Header("Lose Target")]
    public float loseTargetTime = 10f;
    private float lastSeenTime;

    private float lastAttackTime;
    private bool isDead = false;
    private float alertTimer;
    private float hitboxActiveTime = 0f;
    private bool isHitboxActive = false;
    private bool isAttacking = false; // 공격 중인지 여부
    private float attackStartTime = 0f; // 공격 시작 시간
    public float attackDuration = 1.0f; // 공격 애니메이션 지속 시간

    void Start()
    {
        Debug.Log("EnemyAI: Start() called on " + gameObject.name);
        
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (anim == null) anim = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();

        if (attackHitbox == null)
        {
            Debug.LogError("EnemyAI: attackHitbox is NULL! Please assign it in Inspector on " + gameObject.name);
        }
        else
        {
            Debug.Log("EnemyAI: attackHitbox is assigned: " + attackHitbox.gameObject.name);
        }

        ChangeState(EnemyState.Idle);
    }

    void Update()
    {
        if (isDead) return;

        // 히트박스 활성화 시간 관리
        if (isHitboxActive)
        {
            hitboxActiveTime -= Time.deltaTime;
            if (hitboxActiveTime <= 0f)
            {
                DisableAttackHitbox();
                isHitboxActive = false;
            }
        }

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

    void Awake()
    {
        Debug.Log("EnemyAI: Awake() called on " + gameObject.name);
    }

    void OnEnable()
    {
        Debug.Log("EnemyAI: OnEnable() called on " + gameObject.name);
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
            Debug.Log("EnemyAI: Player in attack range (" + dist.ToString("F2") + " <= " + attackRange + "), changing to Attack state");
            ChangeState(EnemyState.Attack);
        }
    }

    void UpdateAttack()
    {
        if (player == null) return;

        // 공격 중에는 항상 정지
        agent.isStopped = true;
        anim.SetBool("isRun", false);

        // 공격 중일 때는 플레이어를 향해 회전만 함
        if (isAttacking)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0f;

            if (dir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
            }

            // 공격 애니메이션이 끝났는지 확인
            if (Time.time >= attackStartTime + attackDuration)
            {
                isAttacking = false;
            }

            return; // 공격 중에는 다른 로직 실행 안 함
        }

        // 공격 중이 아닐 때만 플레이어를 향해 회전
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        dirToPlayer.y = 0f;

        if (dirToPlayer != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dirToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
        }

        float dist = Vector3.Distance(transform.position, player.position);

        // 플레이어가 공격 범위를 벗어나면 추적 상태로 변경
        if (dist > attackRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        // 공격 쿨다운이 끝났으면 공격 시작
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            attackStartTime = Time.time;
            isAttacking = true;
            
            Debug.Log("EnemyAI: Attack cooldown ready, triggering attack animation");
            anim.SetTrigger("Attack");
            // 공격 시작 시 히트박스 활성화
            Debug.Log("EnemyAI: Attack triggered, enabling hitbox for " + hitboxActiveDuration + " seconds");
            EnableAttackHitbox();
            hitboxActiveTime = hitboxActiveDuration;
            isHitboxActive = true;
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
        // 상태 변경 시 히트박스 비활성화 및 공격 상태 초기화
        if (currentState == EnemyState.Attack && newState != EnemyState.Attack)
        {
            DisableAttackHitbox();
            isHitboxActive = false;
            isAttacking = false; // 공격 상태 초기화
        }

        currentState = newState;

        if (newState == EnemyState.Idle)
        {
            agent.isStopped = true;
        }
    }


    public void TakeDamage()
    {
        if (isDead) return;

        // 피격 시 히트박스 비활성화 및 공격 상태 초기화
        DisableAttackHitbox();
        isHitboxActive = false;
        isAttacking = false; // 공격 중단

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
    public void EnableAttackHitbox()
    {
        if (attackHitbox != null)
        {
            Debug.Log("EnemyAI: Enabling attack hitbox");
            attackHitbox.EnableHitbox();
        }
        else
        {
            Debug.LogError("EnemyAI: attackHitbox is null! Please assign it in the Inspector.");
        }
    }

    public void DisableAttackHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.DisableHitbox();
    }
    public void FootR()
    {

    }
    public void FootL()
    {

    }
}
