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
    public bool isAware = false;

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
            isAware = true;
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Enemy Dead");

        Destroy(gameObject, 2f);
    }
}
