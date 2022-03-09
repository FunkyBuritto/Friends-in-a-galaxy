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
            Minimap.UnsubscribeEnemy(transform);
            Destroy(gameObject);
        }
    }
}
