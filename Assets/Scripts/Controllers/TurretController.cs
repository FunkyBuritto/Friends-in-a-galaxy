using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

public class TurretController : MonoBehaviour
{
    public bool useKeyboard = false;
    public float rotationSpeed;

    public Transform shootPoint;
    public float attackDelay;
    public float projectileSpeed;
    public GameObject projectile;

    float prevTime;
    float currTime;

    float inputValue = 0;
    bool shooting = false;
    bool addedUser = false;
    OSCUser user;

    // Start is called before the first frame update
    void Start()
    {
        //user = OSCUser.GetGunner();
    }

    // Update is called once per frame
    void Update()
    {
        if (useKeyboard)
        {
            inputValue = Input.GetAxisRaw("Horizontal");
            shooting = Input.GetKey(KeyCode.Space);
        }
        else
        {
            if (!addedUser)
            {
                user = OSCUser.GetDriver();
                addedUser = true;
            }
            else
            {
                user.AddHook("gravity", setInput);
                user.AddHook("touch0", setTouch);
            }
        }
        
        if (Mathf.Abs(inputValue) < 0.1)
            return;

        RotateTurret(inputValue);
    }

    public void RotateTurret(float value) { transform.Rotate(new Vector3(0, 0, inputValue * rotationSpeed * Time.deltaTime)); }

    public void setInput(ArrayList list) { inputValue = (float)list[0]; }
    public void setTouch(ArrayList list) {
        currTime = Time.time;
        if (currTime - prevTime < attackDelay)
            return;
        GameObject proj = Instantiate(projectile);
        Rigidbody2D rigidbd = proj.GetComponent<Rigidbody2D>();
        proj.transform.position = shootPoint.position;
        proj.transform.rotation = transform.rotation;
        
        rigidbd.AddForceAtPosition(proj.transform.up.normalized * projectileSpeed, transform.position, ForceMode2D.Impulse);
        prevTime = currTime;
    }

}
