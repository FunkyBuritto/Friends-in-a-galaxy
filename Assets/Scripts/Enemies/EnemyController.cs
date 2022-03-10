using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EnemyBase
{
    public override void Relocating()
    {
        base.Relocating();
    }

    public override void Aiming()
    {
        base.Aiming();
    }

    public override void Shoot()
    {
        base.Shoot();
    }

    public override IEnumerator Reloading()
    {
        yield return base.Reloading();
    }

    private void Start()
    {
        Minimap.SubscribeEnemy(transform, Minimap.EnemyTypes.Default);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
            Hp--;

        if (Hp <= 0)
        {
            if (Random.Range(0f, 100f) < dropChance)
            {
                Debug.Log("dropped");
                Instantiate(upgrade, transform.position, Quaternion.identity);
            }
                
            Minimap.UnsubscribeEnemy(transform);
            Destroy(gameObject);
        }
    }
}
