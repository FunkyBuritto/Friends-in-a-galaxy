using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    protected GameObject player;
    protected Rigidbody2D rb;
    protected bool boosting = false;

    protected float aimStartTime;
    protected float rotateLerp = 0;

    public bool isAiming = false, isReloading = false;

    public ParticleSystem ps;
    public GameObject projectile;
    public float projectileSpeed;
    public float reloadSpeed;
    public float rotationSpeed;
    public float movementSpeed;
    public int Hp = 3;

    public float idleDist = 30;
    public float attackDist = 12;
    public float backingDist = 6;

    public enum FightState
    {
        Relocating,
        Aiming,
        Shooting,
        Reloading,
        Idle
    }
    public FightState state = FightState.Idle;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (boosting && !ps.isPlaying)
        {
            ps.Play();
        }
        else if (!boosting && ps.isPlaying)
        {
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
                if (!isAiming)
                    aimStartTime = Time.time;
                Aiming();
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

    public virtual void Relocating()
    {
        float distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.y / 6 * 8), player.transform.position);
        if (distance > idleDist)
        {
            // maybe return to group
            // state = FightState.Idle;
        }
        else if (distance < backingDist)
        {
            // Rotate towards player
            rotateLerp += Time.deltaTime * rotationSpeed;
            var dir = transform.position - player.transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float lerpedAngle = Mathf.LerpAngle(transform.eulerAngles.z, angle + 90, rotateLerp);
            transform.rotation = Quaternion.AngleAxis(lerpedAngle, Vector3.forward);
            // Go away from player
            rb.AddForce((transform.position - player.transform.position).normalized * movementSpeed * Time.deltaTime);
        }
        else if (distance < attackDist)
        {
            rotateLerp = 0;
            state = FightState.Aiming;
        }
        else if (distance <= idleDist)
        {
            // Rotate towards player
            rotateLerp += Time.deltaTime * rotationSpeed;
            var dir = transform.position - player.transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float lerpedAngle = Mathf.LerpAngle(transform.eulerAngles.z, angle + 90, rotateLerp);
            transform.rotation = Quaternion.AngleAxis(lerpedAngle, Vector3.forward);

            // Move towards player
            rb.AddForce((transform.position - player.transform.position).normalized * -movementSpeed * Time.deltaTime);
        }
    }

    public virtual void Aiming()
    {
        isAiming = true;
        rotateLerp += Time.deltaTime * rotationSpeed;
        var dir = transform.position - player.transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float lerpedAngle = Mathf.LerpAngle(transform.eulerAngles.z, angle + 90, rotateLerp);
        transform.rotation = Quaternion.AngleAxis(lerpedAngle, Vector3.forward);

        if (Time.time - aimStartTime > 0.5f)
        {
            isAiming = false;
            state = FightState.Shooting;
            rotateLerp = 0;
        }
    }

    public virtual void Shoot()
    {
        GameObject proj = Instantiate(projectile);
        Rigidbody2D rigidbd = proj.GetComponent<Rigidbody2D>();
        proj.transform.position = transform.position;
        proj.transform.rotation = transform.rotation;
        rigidbd.AddForceAtPosition(proj.transform.up.normalized * projectileSpeed, transform.position, ForceMode2D.Impulse);
        state = FightState.Reloading;
    }

    public virtual IEnumerator Reloading()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadSpeed);
        isReloading = false;
        state = FightState.Relocating;
    }
}
