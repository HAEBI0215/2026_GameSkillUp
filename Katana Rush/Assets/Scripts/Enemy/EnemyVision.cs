using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("Vision Settings")]
    public float viewDistance = 10f;
    public float viewAngle = 90f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    private Enemy enemy;

    void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        CheckVision();
    }

    void CheckVision()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, viewDistance, playerLayer);

        if (targets.Length == 0)
        {
            enemy.isAware = false;
            return;
        }

        Transform player = targets[0].transform;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (angle < viewAngle / 2f)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            // 장애물 검사 (벽 있으면 안보임)
            if (!Physics.Raycast(transform.position + Vector3.up, dirToPlayer, distance, obstacleLayer))
            {
                enemy.isAware = true;
                Debug.Log("Player Detected!");
                return;
            }
        }

        enemy.isAware = false;
    }

    // 씬뷰에서 시야 확인용
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}
