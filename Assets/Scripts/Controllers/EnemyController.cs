using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    public GameObject player;
    public enum FightState
    {
        Relocating,
        Aiming,
        Shooting,
        Reloading,
        Idle
    }
    public FightState state = FightState.Idle;
    Rigidbody2D rb;

    public float movementSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case FightState.Relocating:
                Relocating();
                break;
            case FightState.Aiming:
                Aiming();
                break;
            case FightState.Shooting:
                break;
            case FightState.Reloading:
                break;
            case FightState.Idle:
                break;
        }


        //Debug.DrawRay(transform.position, transform.up * 10 + transform.right * 8, Color.red);
        //Debug.DrawRay(transform.position, transform.up * 10 + transform.right * 4, Color.red);
        //Debug.DrawRay(transform.position, transform.up * 10, Color.red);
        //Debug.DrawRay(transform.position, transform.up * 10 + transform.right * -4, Color.red);
        //Debug.DrawRay(transform.position, transform.up * 10 + transform.right * -8, Color.red);

    }

    void Relocating()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance > 20){
            // maybe return to group
            state = FightState.Idle;
            
        }
        else if(distance < 6){
            // Go away from player
            rb.AddForceAtPosition(transform.position.normalized * movementSpeed * Time.deltaTime, player.transform.position);
        }
        else if (distance < 12){
            state = FightState.Aiming;
        }
        else if (distance <= 20){
            // Move towards player
            rb.AddForceAtPosition(-transform.position.normalized * movementSpeed * Time.deltaTime, -player.transform.position);
        }
    }

    void Aiming()
    {
        var dir = transform.position - player.transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }
}
