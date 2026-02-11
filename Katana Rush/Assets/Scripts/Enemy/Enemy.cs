using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("HP")]
    public float maxHp = 100f;
    public float currentHp;

    [Header("Damage Settings")]
    public float backAttackMultiplier = 1.5f;

    private bool isDead;

    [Header("State")]
    public bool isAware = false; // 플레이어 인식 여부

    void Start()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(float damage, bool isBackAttack)
    {
        if (isDead) return;

        float finalDamage = damage;

        if (!isAware && isBackAttack)
        {
            finalDamage *= backAttackMultiplier;
            Debug.Log("백어택 : " + finalDamage);
        }

        currentHp -= finalDamage;

        Debug.Log($"Enemy Hit! Damage: {finalDamage}, HP: {currentHp}");

        if (currentHp <= 0f)
        {
            Die();
        }
        else
        {
            // 맞았으니까 이제 인식하게 만들기
            isAware = true;
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Enemy Dead");

        // TODO: 애니메이션/콜라이더/드랍 등
        Destroy(gameObject, 2f);
    }
}
