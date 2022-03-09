using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public static Portal portal;

    public List<Vector3> linePoints = new List<Vector3>();
    public float pullPower;
    public float blockRadius;
    public int blockPointAmount;
    public float blockFreq;
    public int eliteCount = 3;

    public bool hasShield = true;

    private Rigidbody2D playerRB;
    private GameObject player;
    private LineRenderer lr;
    public GameObject shield;

    // Start is called before the first frame update
    void Start()
    {
        portal = this;
        lr = GetComponentInChildren<LineRenderer>();

        // Generate Points
        float angle = 360 / blockPointAmount;
        for (int i = 0; i < 360; i += (int)angle) {
            linePoints.Add(new Vector3(Mathf.Sin(i * Mathf.PI / 180), Mathf.Cos(i * Mathf.PI / 180), 0) * blockRadius);
        }

        lr.positionCount = blockPointAmount;
        lr.SetPositions(linePoints.ToArray());
    }

    public void DisableShield()
    {
        shield.SetActive(false);
        hasShield = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log(hasShield);
        if (hasShield)
        {
            for (int i = 0; i < linePoints.Count; i++)
            {
                Debug.Log("changing");
                lr.SetPosition(i, linePoints[i] + Random.insideUnitSphere * blockFreq);
            }

            if (playerRB)
                playerRB.AddForce(-(player.transform.position - transform.position).normalized * pullPower);
            if (player && Vector2.Distance(player.transform.position, transform.position) < 1)
                Debug.Log("Inside Portal");
        }

        if (eliteCount <= 0) DisableShield();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject;
            playerRB = player.GetComponent<Rigidbody2D>();
        }
    }
}
