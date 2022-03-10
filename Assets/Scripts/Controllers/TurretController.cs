using System.Collections;
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
    [HideInInspector] public bool addedUser = false;
    OSCUser user;

    private void Update()
    {
        if (Overlay.isBlocking) return;

        if (useKeyboard)
        {
            if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.LeftArrow)) inputValue = 0;
            inputValue = Input.GetKey(KeyCode.LeftArrow) ? 1 : inputValue;
            inputValue = Input.GetKey(KeyCode.RightArrow) ? -1 : inputValue;
            shooting = Input.GetMouseButton(0);
        }
        else
        {
            if (!addedUser)
            {
                user = OSCUser.GetGunner();
                addedUser = true;
            }
            else
            {
                user.AddHook("gravity", SetInput);
                user.AddHook("touch0", SetTouch);
            }
        }

        if (shooting) SetTouch(new ArrayList());

        if (Mathf.Abs(inputValue) < 0.1)
            return;

        RotateTurret();
    }

    public void RotateTurret() => transform.Rotate(new Vector3(0, 0, inputValue * rotationSpeed * Time.deltaTime));

    public void SetInput(ArrayList list) => inputValue = (float)list[0];
    public void SetTouch(ArrayList list) {
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
