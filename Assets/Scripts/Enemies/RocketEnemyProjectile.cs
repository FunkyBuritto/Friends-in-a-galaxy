using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketEnemyProjectile : MonoBehaviour
{
    public GameObject particle;
    public int damage;

    [HideInInspector] public GameObject instatiator;
    [HideInInspector] public Transform target = null;

    public bool inverted = false;

    public float trajectorySpeed;
    public float trajectoryFreq;
    public float trajectoryRange;

    public float homingSpeed;
    public float lockedOnSpeed = 15;
    Rigidbody2D rb;

    Vector2 forward;
    float homingLerp = 0;
    float trajectoryLerp = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        forward = transform.up;
        StartCoroutine(DestroyAfter(10f));
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
            ShipController.PlayerShip.Hp -= damage;

        Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (target == null && coll.gameObject.CompareTag("Player")) target = coll.gameObject.transform;
    }

    private void Update()
    {
        if (target != null)
        {
            homingLerp += Time.deltaTime * homingSpeed;
            var dir = transform.position - target.transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float lerpedAngle = Mathf.LerpAngle(transform.eulerAngles.z, angle + 90, homingLerp);
            transform.rotation = Quaternion.AngleAxis(lerpedAngle, Vector3.forward);
            rb.velocity = transform.up.normalized * lockedOnSpeed;
        }
        else
        {
            trajectoryLerp += Time.deltaTime * trajectoryFreq;
            var angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(Mathf.Sin(inverted ? -trajectoryLerp - 2 : trajectoryLerp + 2) * trajectoryRange + angle - 90, Vector3.forward);
            rb.velocity = transform.up.normalized * trajectorySpeed;
        }
    }

    IEnumerator DestroyAfter(float time)
    {
        yield return new WaitForSeconds(time);
        Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
