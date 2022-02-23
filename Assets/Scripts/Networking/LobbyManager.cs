using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityOSC;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;

    /// <summary> List of the connected users. </summary>
    [SerializeField] public List<OSCUser> users = new List<OSCUser>();

    /// Events
    public delegate void ConnectionEvent(OSCUser user, int index);
    public event ConnectionEvent OnConnection;
    public delegate void DisconnectEvent(int index, string ip);
    public event DisconnectEvent OnDisconnect;

    private void Awake()
    {
        // Make sure we don't get doubles:
        if (FindObjectsOfType<LobbyManager>().Length == 1)
        {
            DontDestroyOnLoad(gameObject);

            instance = this;
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Add a global hook.
        OSCHandler.AddGlobalHook(OnMessage);
    }

    /// <summary>
    /// Start the game by loading the game scene.
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene(Constants.GAMESCENE);
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
        users.Add(new OSCUser(ip));
        if (OnConnection != null) OnConnection.Invoke(users[users.Count - 1], users.Count - 1);
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
        if (OnDisconnect != null) OnDisconnect.Invoke(users.IndexOf(user), user.ip);
        users.Remove(user); // Remove the user once they time out.
    }
}
