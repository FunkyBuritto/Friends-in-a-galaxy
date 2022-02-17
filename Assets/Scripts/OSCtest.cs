using UnityEngine;
using UnityOSC;

public class OSCtest : MonoBehaviour
{
    [SerializeField] private OSC osc;

    private void Start()
    {
        osc.SetAddressHandler("/Device/gravity", Receive);
    }

    void Receive(OscMessage msg)
    {
        float x = msg.GetFloat(0);
        Debug.Log($"[{msg.ip}]: " + x);
    }
}