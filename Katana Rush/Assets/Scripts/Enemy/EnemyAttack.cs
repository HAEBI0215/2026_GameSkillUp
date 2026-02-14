using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public Animator anim;

    [Header("Swords")]
    public EnemyHitbox[] swords;

    [Header("Attack Settings")]
    public float attackCooldown = 0.6f;

    public float hitboxOnTime = 0.15f;
    public float hitboxOffTime = 0.35f;

    private bool canAttack = true;

    public GameObject hitVFX;
    public Transform hitPoint;

    void Update()
    {
        // 플레이어 입력에 반응하지 않도록 비활성화
        // 적의 공격은 EnemyAI에서 관리합니다
        // if (Input.GetMouseButtonDown(0) && canAttack)
        // {
        //     StartCoroutine(AttackRoutine());
        // }
    }

    IEnumerator AttackRoutine()
    {
        canAttack = false;

        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(hitboxOnTime);

        foreach (EnemyHitbox EH in swords)
        {
            EH.EnableHitbox();
        }

        yield return new WaitForSeconds(hitboxOffTime - hitboxOnTime);

        foreach (EnemyHitbox EH in swords)
        {
            EH.DisableHitbox();
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
