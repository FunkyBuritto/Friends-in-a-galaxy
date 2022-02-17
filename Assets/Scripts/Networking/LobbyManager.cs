using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;

    /// <summary> List of the connected users. </summary>
    [SerializeField] public List<OSCUser> users = new List<OSCUser>();

    /// Events
    public delegate void ConnectionEvent(string addr, int index);
    public event ConnectionEvent OnConnection;
    public delegate void DisconnectEvent(int index);
    public event DisconnectEvent OnDisconnect;

    private void Awake()
    {
        // Make sure we don't get doubles:
        if (FindObjectsOfType<LobbyManager>().Length == 1)
        {
            DontDestroyOnLoad(gameObject);

            instance = this;
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Add a global hook.
        OSCHandler.AddGlobalHook(OnMessage);
    }

    /// <summary>
    /// Called every time the OSC server recieves a message.
    /// </summary>
    private void OnMessage(OscMessage msg)
    {
        // If the user is new call on connect.
        if (UserExists(msg.ip) == false) OnConnect(msg.ip);
        else ResetTimeout(users.Find(u => u.ip == msg.ip));
    }

    /// <summary>
    /// Called once a new user sends their first message.
    /// </summary>
    private void OnConnect(string ip)
    {
        OnConnection.Invoke(ip, users.Count);
        users.Add(new OSCUser(ip));
    }

    /// <summary>
    /// Check whether a user already exists on the network.
    /// </summary>
    /// <param name="ip">The ip to look for</param>
    /// <returns>If the user already exists on the network.</returns>
    private bool UserExists(string ip)
    {
        for (int i = 0; i < users.Count; i++)
            if (users[i].ip == ip) return true;
        return false;
    }

    /// <summary>
    /// Reset the timeout for a user (2 seconds)
    /// </summary>
    public void ResetTimeout(OSCUser user)
    {
        StopCoroutine(nameof(Timeout));
        StartCoroutine(nameof(Timeout), user);
    }

    private IEnumerator Timeout(OSCUser user)
    {
        yield return new WaitForSeconds(2f);
        OnDisconnect.Invoke(users.IndexOf(user));
        users.Remove(user); // Remove the user once they time out.
    }
}