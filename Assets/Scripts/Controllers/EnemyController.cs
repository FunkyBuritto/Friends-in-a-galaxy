using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    public GameObject player;
    public GameObject projectile;
    public float projectileSpeed;
    public enum FightState
    {
        Relocating,
        Aiming,
        Shooting,
        Reloading,
        Idle
    }
    public FightState state = FightState.Idle;
    public float movementSpeed;
    public bool isAiming = false, isReloading = false;
    public ParticleSystem ps;

    bool boosting = false;
    Rigidbody2D rb;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (boosting && !ps.isPlaying) {
            ps.Play();
        }
        else if (!boosting && ps.isPlaying) {
            ps.Stop();
        }
        switch (state)
        {
            case FightState.Relocating:
                boosting = true;
                Relocating();
                break;
            case FightState.Aiming:
                boosting = false;
                if(!isAiming)
                StartCoroutine(Aiming());
                break;
            case FightState.Shooting:
                boosting = false;
                Shoot();
                break;
            case FightState.Reloading:
                boosting = false;
                if (!isReloading)
                    StartCoroutine(Reloading());
                break;
            case FightState.Idle:
                boosting = false;
                break;
        }
    }

    void Relocating()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance > 30){
            // maybe return to group
            // state = FightState.Idle;
        }
        else if(distance < 6){
            // Go away from player
            rb.AddForce((transform.position - player.transform.position).normalized * movementSpeed * Time.deltaTime);
        }
        else if (distance < 12){
            state = FightState.Aiming;
        }
        else if (distance <= 30){
            // Move towards player
            var dir = transform.position - player.transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
            rb.AddForce((transform.position - player.transform.position).normalized * -movementSpeed * Time.deltaTime);
        }
    }

    IEnumerator Aiming()
    {
        isAiming = true;
        var dir = transform.position - player.transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
        yield return new WaitForSeconds(0.5f);
        isAiming = false;
        state = FightState.Shooting;
    }
    
    void Shoot()
    {
        GameObject proj = Instantiate(projectile);
        Rigidbody2D rigidbd = proj.GetComponent<Rigidbody2D>();
        proj.transform.position = transform.position;
        proj.transform.rotation = transform.rotation;
        rigidbd.AddForceAtPosition(proj.transform.up.normalized * projectileSpeed, transform.position, ForceMode2D.Impulse);
        state = FightState.Reloading;
    }

    IEnumerator Reloading()
    {
        isReloading = true;
        yield return new WaitForSeconds(0.8f);
        isReloading = false;
        state = FightState.Relocating;
    }
}
