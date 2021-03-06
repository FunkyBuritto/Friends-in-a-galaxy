using Cinemachine;
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
    public int blockLayers = 1;
    public int eliteCount = 3;

    public bool hasShield = true;

    private Rigidbody2D playerRB;
    private GameObject player;
    private LineRenderer lr;
    private CinemachineVirtualCamera cam;
    public GameObject shield;

    // Start is called before the first frame update
    void Start()
    {
        portal = this;
        cam = GameObject.FindWithTag("PortalCam").GetComponent<CinemachineVirtualCamera>();
        lr = GetComponentInChildren<LineRenderer>();

        // Generate Points
        float angle = 360 / blockPointAmount;
        for (int i = 0; i < (360 * blockLayers); i += (int)angle) {
            linePoints.Add(new Vector3(Mathf.Sin(i * Mathf.PI / 180), Mathf.Cos(i * Mathf.PI / 180), 0) * blockRadius + transform.position);
        }

        lr.positionCount = blockPointAmount * blockLayers;
        lr.SetPositions(linePoints.ToArray());
    }

    public void DisableShield()
    {
        shield.SetActive(false);
        if (hasShield == true)
        {
            hasShield = false;
            StartCoroutine(nameof(DisableShieldCutScene));
        }
    }

    IEnumerator DisableShieldCutScene()
    {
        cam.ForceCameraPosition(new Vector3(transform.position.x, transform.position.y, -10), Quaternion.identity);
        cam.Priority = 11;
        yield return new WaitForSeconds(4.0f);
        cam.Priority = 9;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (hasShield) {
            for (int i = 0; i < linePoints.Count; i++) {
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
