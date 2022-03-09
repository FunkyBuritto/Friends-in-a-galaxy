using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private Rigidbody2D playerRB;
    private GameObject player;
    private LineRenderer lr;

    public List<Vector3> linePoints = new List<Vector3>();
    public float pullPower;
    public float blockRadius;
    public int blockPointAmount;
    public float blockFreq;

    [HideInInspector] public bool hasShield = true;

    // Start is called before the first frame update
    void Start()
    {
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
        lr.GetComponent<GameObject>().SetActive(false);
        hasShield = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (hasShield)
        {
            for (int i = 0; i < linePoints.Count; i++)
            {
                lr.SetPosition(i, linePoints[i] + Random.insideUnitSphere * blockFreq);
            }

            if (playerRB)
                playerRB.AddForce(-(player.transform.position - transform.position).normalized * pullPower);
            if (player && Vector2.Distance(player.transform.position, transform.position) < 1)
                Debug.Log("Inside Portal");
        }
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
