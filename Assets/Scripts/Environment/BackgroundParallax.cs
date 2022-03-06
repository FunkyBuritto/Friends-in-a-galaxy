using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    public float parallaxSpeed;
    private Rigidbody2D playerBody;

    void Start() {
        playerBody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        transform.Translate(playerBody.velocity * parallaxSpeed);
    }
}
