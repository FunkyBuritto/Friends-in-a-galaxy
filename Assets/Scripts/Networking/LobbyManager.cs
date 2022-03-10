using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityOSC;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;

    /// <summary> List of the connected users. </summary>
    [SerializeField] public Dictionary<GUID, OSCUser> users = new Dictionary<GUID, OSCUser>();

    /// Events
    public delegate void ConnectionEvent(OSCUser user);
    public event ConnectionEvent OnConnection;
    public delegate void DisconnectEvent(OSCUser user);
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
        else ResetTimeout(users.Values.First(u => u.ip == msg.ip));
    }

    /// <summary>
    /// Called once a new user sends their first message.
    /// </summary>
    private void OnConnect(string ip)
    {
        UserRole role;
        if (users.Count == 0 || users.Values.Any(u => u.role != UserRole.Driver)) role = UserRole.Driver;
        else if (users.Values.Any(u => u.role != UserRole.Gunner)) role = UserRole.Gunner;
        else role = UserRole.Spectator;

        OSCUser user = new OSCUser(ip, role);
        users.Add(user.id, user);
        user.timeout = StartCoroutine(Timeout(user));
        if (OnConnection != null) OnConnection.Invoke(user);
    }

    /// <summary>
    /// Check whether a user already exists on the network.
    /// </summary>
    /// <param name="ip">The ip to look for</param>
    /// <returns>If the user already exists on the network.</returns>
    private bool UserExists(string ip)
    {
        return users.Values.Any(u => u.ip == ip);
    }

    /// <summary>
    /// Remove a user from the game.
    /// </summary>
    /// <param name="user">The user object.</param>
    public void RemoveUser(OSCUser user)
    {
        if (OnDisconnect != null) OnDisconnect.Invoke(user);
        Debug.LogWarning($"{user.ip} Disconnected!");
        users.Remove(user.id);
    }

    /// <summary>
    /// Reset the timeout for a user (2 seconds)
    /// </summary>
    public void ResetTimeout(OSCUser user)
    {
        StopCoroutine(user.timeout);
        user.timeout = StartCoroutine(Timeout(user));
    }

    private IEnumerator Timeout(OSCUser user)
    {
        yield return new WaitForSeconds(2f);
        RemoveUser(user);
    }
}
