using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

public class BoosterController : MonoBehaviour
{
    public bool useKeyboard = false;
    public float boostSpeed;
    public float rotationSpeed;
    public float maxRotation;

    float inputValue = 0;
    bool boosting = false;
    bool addedUser = false;

    Rigidbody2D shipBody;
    Transform spriteTransform;
    OSCUser user;

    // Start is called before the first frame update
    void Start()
    {
        shipBody = GetComponentInParent<Rigidbody2D>();
        spriteTransform = GetComponentsInChildren<Transform>()[1];
    }

    // Update is called once per frame
    void Update()
    {
        if (useKeyboard) {
            inputValue = Input.GetAxisRaw("Horizontal");
            boosting = Input.GetKey(KeyCode.Space);
        }
        else {
            if (!addedUser)
            {
                user = OSCUser.GetDriver();
                addedUser = true;
            }
            else
            {
                user.AddHook("gravity", setInput);
                user.AddHook("touch", setTouch);
            }
        }
    }

    private void FixedUpdate()
    {
        if (boosting && useKeyboard) {
            shipBody.AddForceAtPosition(transform.up.normalized * boostSpeed, spriteTransform.position, ForceMode2D.Impulse);
        }

        if (!(Mathf.Abs(inputValue) < 0.1))
            RotateBooster(inputValue);
    }
    public void RotateBooster(float value) { transform.Rotate(new Vector3(0, 0, -inputValue * rotationSpeed * Time.deltaTime)); }

    public void setInput(ArrayList list) { inputValue = (float)list[0]; }
    public void setTouch(ArrayList list) { shipBody.AddForceAtPosition(transform.up.normalized * boostSpeed, spriteTransform.position, ForceMode2D.Impulse); }
}
