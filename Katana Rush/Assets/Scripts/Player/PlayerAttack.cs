using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    public Animator anim;

    [Header("Swords")]
    public SwordHitbox[] swords;

    [Header("Attack Settings")]
    public float attackCooldown = 0.6f;

    public float hitboxOnTime = 0.15f;
    public float hitboxOffTime = 0.35f;

    private bool canAttack = true;

    public GameObject hitVFX;
    public Transform hitPoint;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        canAttack = false;

        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(hitboxOnTime);

        foreach (SwordHitbox sword in swords)
        {
            sword.EnableHitbox();
        }

        yield return new WaitForSeconds(hitboxOffTime - hitboxOnTime);

        foreach (SwordHitbox sword in swords)
        {
            sword.DisableHitbox();
        }

        yield return new WaitForSeconds(attackCooldown - hitboxOffTime);
        canAttack = true;
    }

    public void Hit()
    {
        if (hitVFX != null && hitPoint != null)
        {
            Instantiate(hitVFX, hitPoint.position, hitPoint.rotation);
        }
    }
}
