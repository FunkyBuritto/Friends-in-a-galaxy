using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteEnemyController : EnemyBase
{
    public override void Relocating()
    {
        if(player == null) player = GameObject.FindGameObjectWithTag("Player");
        if(rb == null) rb = GetComponent<Rigidbody2D>();
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

    private void Awake()
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
            
        if (collision.gameObject.CompareTag("Projectile") && Vector2.Distance(collision.transform.position, collision.otherCollider.transform.position) < 1.925f)
        {
            Hp--;
        }
            

        if (Hp <= 0) {
            if (Random.Range(0f, 100f) < dropChance)
                Instantiate(upgrade, transform.position, Quaternion.identity);

            Instantiate(deathParticle, transform.position, Quaternion.identity);
            Minimap.UnsubscribeEnemy(transform);
            Portal.portal.eliteCount--;
            Destroy(gameObject);
        }      
    }
}
