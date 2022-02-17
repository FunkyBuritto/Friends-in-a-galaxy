using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

public class BoosterController : MonoBehaviour
{
    [HideInInspector] public float inputValue = 0;

    public bool useKeyboard = false;
    public float rotationSpeed;
    public float maxRotation;

    bool boosting = false;
    bool addedUser = false;
    OSCUser user;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (useKeyboard) {
            inputValue = Input.GetAxisRaw("Horizontal");
            boosting = Input.GetKey(KeyCode.Space);
        }
        else {
            if(!addedUser)
                user = OSCUser.GetDriver();
            else
            {
                user.AddHook("gravity", setInput);
            }
        }

        if (Mathf.Abs(inputValue) < 0.1)
            return;

        RotateBooster(inputValue);
    }

    public void setInput(ArrayList list)
    {
        inputValue = (float)list[0];
    }

    public void RotateBooster(float value) { transform.Rotate(new Vector3(0, 0, -inputValue * rotationSpeed * Time.deltaTime)); }

}
