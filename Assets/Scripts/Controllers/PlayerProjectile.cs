using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public GameObject particle;
    public Transform target = null;
    public float homingSpeed;
    public float lockedOnSpeed = 15;
    Rigidbody2D rb;
    float lerp = 0;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(DestroyAfter(1.8f));
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.CompareTag("Enemy") || coll.gameObject.CompareTag("Obstacle"))
        {
            Instantiate(particle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (target == null && coll.gameObject.CompareTag("Enemy")) target = coll.gameObject.transform;
    }

    private void Update()
    {
        if(target != null)
        {
            lerp += Time.deltaTime * homingSpeed;
            var dir = transform.position - target.transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float lerpedAngle = Mathf.LerpAngle(transform.eulerAngles.z, angle + 90, lerp);
            transform.rotation = Quaternion.AngleAxis(lerpedAngle, Vector3.forward);
            rb.velocity = transform.up.normalized * lockedOnSpeed;
        }
    }

    IEnumerator DestroyAfter(float time)
    {
        yield return new WaitForSeconds(time);
        Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
