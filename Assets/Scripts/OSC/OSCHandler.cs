using UnityEngine;
using UnityOSC;

[RequireComponent(typeof(OSC))]
public class OSCHandler : MonoBehaviour
{
    public static OSC instance;

    private void Awake()
    {
        // Make sure we don't get doubles:
        if (FindObjectsOfType<OSC>().Length == 1)
        {
            DontDestroyOnLoad(gameObject);

            instance = GetComponent<OSC>();
        }
        else Destroy(gameObject);
    }

    /// <summary>
    /// Add a hook into the OSC server that only triggers when its from a specific ip.
    /// </summary>
    /// <param name="address">The OSC address /Device/{type}</param>
    /// <param name="ip">The IP address to look for</param>
    /// <param name="handler">The callback for when a message is matched</param>
    public static void AddUserHook(string address, string ip, OscMessageHandler handler)
    {
        // Only invoke the handler if the ip matches:
        instance.SetAddressHandler(address, (msg) => { 
            if (msg.ip == ip) handler.Invoke(msg);
        });
    }

    /// <summary>
    /// Add a hook into the OSC server that triggers on EVERY message.
    /// </summary>
    /// <param name="handler"></param>
    public static void AddGlobalHook(OscMessageHandler handler)
    {
        instance.SetAllMessageHandler(handler);
    }
}
