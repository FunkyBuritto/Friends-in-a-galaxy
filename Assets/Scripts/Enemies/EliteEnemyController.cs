using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteEnemyController : EnemyBase
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

    private void Start()
    {
        Minimap.SubscribeEnemy(transform, Minimap.EnemyTypes.Guardian);
    }

    public override IEnumerator Reloading()
    {
        yield return base.Reloading();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.otherCollider.CompareTag("Enemy"))
        {
            GameObject shield = collision.otherCollider.gameObject;
            LeanTween.alpha(shield, 1, 0.15f).setOnComplete(() => LeanTween.alpha(shield, 0.6862f, 0.05f));
            return;
        }
            
        if (collision.gameObject.CompareTag("Projectile"))
            Hp--;

        if (Hp <= 0) {
            Minimap.UnsubscribeEnemy(transform);
            Portal.portal.eliteCount--;
            Destroy(gameObject);
        }      
    }
}
