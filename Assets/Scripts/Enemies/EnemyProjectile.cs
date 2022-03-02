using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public GameObject particle;
    public int damage;

    private void Start()
    {
        StartCoroutine(DestroyAfter(5f));
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
            ShipController.PlayerShip.Hp -= damage;

        Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    IEnumerator DestroyAfter(float time)
    {
        yield return new WaitForSeconds(time);
        Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
