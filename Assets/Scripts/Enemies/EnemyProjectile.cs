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
        if (coll.gameObject.CompareTag("Player") && Vector2.Distance(coll.transform.position, coll.otherCollider.transform.position) < 1.9f){
            ShipController.PlayerShip.Hp -= damage;
        }
            

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
