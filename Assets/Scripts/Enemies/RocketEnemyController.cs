using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketEnemyController : EnemyBase
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
        GameObject proj;
        
        proj = Instantiate(projectile);
        proj.transform.position = transform.position + transform.right.normalized * 0.1f;
        proj.transform.rotation = transform.rotation;
        proj.GetComponent<Rigidbody2D>().AddForceAtPosition(proj.transform.up.normalized * projectileSpeed, transform.position, ForceMode2D.Impulse);

        proj = Instantiate(projectile);
        proj.GetComponent<RocketEnemyProjectile>().inverted = true;
        proj.transform.position = transform.position - transform.right.normalized * 0.1f;
        proj.transform.rotation = transform.rotation;
        proj.GetComponent<Rigidbody2D>().AddForceAtPosition(proj.transform.up.normalized * projectileSpeed, transform.position, ForceMode2D.Impulse);

        state = FightState.Reloading;
    }

    public override IEnumerator Reloading()
    {
        yield return base.Reloading();
    }

    private void Start()
    {
        Minimap.SubscribeEnemy(transform, Minimap.EnemyTypes.Rocket);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile") && Vector2.Distance(collision.transform.position, collision.otherCollider.transform.position) < 1.925f)
            Hp--;

        if (Hp <= 0)
        {
            if (Random.Range(0f, 100f) < dropChance)
                Instantiate(upgrade, transform.position, Quaternion.identity);

            Instantiate(deathParticle, transform.position, Quaternion.identity);
            Minimap.UnsubscribeEnemy(transform);
            Destroy(gameObject);
        }
    }
}
