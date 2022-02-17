using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    [HideInInspector] public float inputValue = 0;

    public bool useKeyboard = false;
    public float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        inputValue = useKeyboard ? Input.GetAxisRaw("Horizontal") : 0;
        if (Mathf.Abs(inputValue) < 0.1)
            return;

        RotateTurret(inputValue);
    }

    public void RotateTurret(float value) { transform.Rotate(new Vector3(0, 0, -inputValue * rotationSpeed * Time.deltaTime)); }

}
