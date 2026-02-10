using UnityEngine;
using System.Collections.Generic;

public class SwordHitbox : MonoBehaviour
{
    public int damage = 10;

    public GameObject impactVFX;
    public float destroyTime = 1f;

    private Collider col;

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    void Start()
    {
        col = GetComponent<Collider>();
        col.enabled = false;
    }

    public void EnableHitbox()
    {
        hitEnemies.Clear();
        col.enabled = true;
    }

    public void DisableHitbox()
    {
        col.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        if (hitEnemies.Contains(other.gameObject)) return;

        hitEnemies.Add(other.gameObject);

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        if (impactVFX != null)
        {
            UnityEngine.Debug.Log("VFX");
            Vector3 hitPos = other.ClosestPoint(transform.position);
            GameObject vfx = Instantiate(impactVFX, hitPos, Quaternion.identity);
            Destroy(vfx, destroyTime);
        }
    }
}
