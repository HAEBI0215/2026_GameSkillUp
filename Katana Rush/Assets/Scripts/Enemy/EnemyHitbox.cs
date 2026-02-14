using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public int damage = 30;

    public GameObject impactVFX;
    public float destroyTime = 1f;

    private Collider col;

    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();

    void Awake()
    {
        Debug.Log("EnemyHitbox: Awake() called on " + gameObject.name);
    }

    void OnEnable()
    {
        Debug.Log("EnemyHitbox: OnEnable() called on " + gameObject.name);
    }

    void Start()
    {
        Debug.Log("EnemyHitbox: Start() called on " + gameObject.name);
        col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
            col.isTrigger = true;
            Debug.Log("EnemyHitbox: Initialized on " + gameObject.name + ", Collider type: " + col.GetType().Name + ", isTrigger: " + col.isTrigger + ", enabled: " + col.enabled);
        }
        else
        {
            Debug.LogError("EnemyHitbox: Collider component not found on " + gameObject.name + ". Please add a Collider component!");
        }
    }

    public void EnableHitbox()
    {
        hitTargets.Clear(); // 이전 히트 기록 초기화
        if (col != null)
        {
            col.enabled = true;
            Debug.Log("EnemyHitbox: Hitbox enabled on " + gameObject.name + ", Collider enabled: " + col.enabled);
        }
        else
        {
            Debug.LogError("EnemyHitbox: Cannot enable hitbox - Collider is null on " + gameObject.name);
        }
    }

    public void DisableHitbox()
    {
        if (col != null)
        {
            col.enabled = false;
        }
    }

    private void ProcessHit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // 이미 이번 공격에서 맞은 타겟인지 확인
        if (hitTargets.Contains(other.gameObject))
        {
            return;
        }
        
        // 타겟을 기록
        hitTargets.Add(other.gameObject);
        Debug.Log("EnemyHitbox: Player hit! Applying damage: " + damage);

        PlayerHealth ph = other.GetComponent<PlayerHealth>();

        if (ph == null)
        {
            ph = other.GetComponentInParent<PlayerHealth>();
        }

        if (ph != null)
        {
            ph.TakeDamage((float)damage); // int를 float로 명시적 변환
        }
        else
        {
            Debug.LogError("EnemyHitbox: PlayerHealth component not found on " + other.gameObject.name);
        }

        // Impact VFX
        if (impactVFX != null)
        {
            Vector3 hitPos = other.ClosestPoint(transform.position);
            GameObject vfx = Instantiate(impactVFX, hitPos, Quaternion.identity);
            Destroy(vfx, destroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("EnemyHitbox: OnTriggerEnter called with " + other.gameObject.name + ", Tag: " + other.tag);
        
        if (!other.CompareTag("Player"))
        {
            Debug.Log("EnemyHitbox: Not a Player tag, ignoring. Tag was: " + other.tag);
            return;
        }

        ProcessHit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        // OnTriggerEnter가 호출되지 않은 경우를 대비 (플레이어가 이미 히트박스 안에 있을 때)
        if (!other.CompareTag("Player")) return;
        
        ProcessHit(other);
    }
}
