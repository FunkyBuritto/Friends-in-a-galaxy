using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

public class BoosterController : MonoBehaviour
{
    public bool useKeyboard = false;
    public float boostSpeed;
    public float maxRotation;
    public ParticleSystem ps;
    public GameObject shield;

    float inputValue = 0;
    bool boosting = false;
    [HideInInspector] public bool addedUser = false;

    float prevTouchTime;

    Rigidbody2D shipBody;
    Transform spriteTransform;
    OSCUser user;

    void Start()
    {
        shipBody = GetComponentInParent<Rigidbody2D>();
        spriteTransform = GetComponentsInChildren<Transform>()[1];
    }

    void Update()
    {
        if (Overlay.isBlocking) return;

        if (useKeyboard) {
            boostSpeed = 0.1f;
            inputValue = Input.GetAxis("Horizontal");
            boosting = Input.GetKey(KeyCode.Space);
        }
        else {
            if (!addedUser) {
                user = OSCUser.GetDriver();
                addedUser = true;
            }
            else {
                user.AddHook("gravity", setInput);
                user.AddHook("touch0", setTouch);
            }
        }
        if (timeTillTouch() > 0.1 && ps.isPlaying)
        {
            ps.Stop();
        }
    }

    private void FixedUpdate()
    {
        if (Overlay.isBlocking) return;

        if (boosting && useKeyboard) {
            shipBody.AddForceAtPosition(transform.up.normalized * boostSpeed, spriteTransform.position, ForceMode2D.Impulse);
        }

        if (useKeyboard)
            RotateBooster(inputValue);
        else
            if (!(Mathf.Abs(inputValue) < 0.1))
                RotateBooster(inputValue);
    }
    public void RotateBooster(float value) {
        float angle = value * maxRotation;
        angle = (angle > 180) ? Mathf.Clamp(angle - 360, -maxRotation, maxRotation) : Mathf.Clamp(angle, -maxRotation, maxRotation);
        transform.localRotation = Quaternion.Euler(0, 0, angle);
        shield.transform.localRotation = Quaternion.Euler(0, 0, -angle);
    }

    public void setInput(ArrayList list) { inputValue = (float)list[0]; }
    public void setTouch(ArrayList list) {
        if (!ps.isPlaying)
            ps.Play();
        shipBody.AddForceAtPosition(transform.up.normalized * boostSpeed * Mathf.Clamp(timeTillTouch(), 0, 0.1f), spriteTransform.position, ForceMode2D.Impulse);
        prevTouchTime = Time.time;
    }

    public float timeTillTouch() => Time.time - prevTouchTime;

}
