using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public GameObject particle;
    private void OnCollisionEnter2D(Collision2D coll)
    {
        StartCoroutine(Destroying());
    }
    
    IEnumerator Destroying()
    {
        Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(gameObject);
        yield return null;
    }
}
