using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHp = 100f;
    public float currentHp;

    public bool isDead = false;

    void Start()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHp -= damage;
        Debug.Log("Player HP : " + currentHp);

        if (currentHp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        currentHp = 0;

        Debug.Log("Player Dead!");
    }
}
