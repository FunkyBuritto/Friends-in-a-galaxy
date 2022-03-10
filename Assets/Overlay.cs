using UnityEngine;
using UnityOSC;

public class Overlay : MonoBehaviour
{
    [SerializeField] private GameObject overlay;
    public static bool isBlocking = false;

    private void Start()
    {
        LobbyManager.instance.OnDisconnect += (_, __) => OnDisconnect();
        LobbyManager.instance.OnConnection += OnConnect;
    }

    private void OnDisconnect()
    {
        overlay.SetActive(true);
        isBlocking = true;
    }

    private void OnConnect(OSCUser user, int index)
    {
        if (LobbyManager.instance.users.Count >= 2)
        {
            overlay.SetActive(false);
            // Force the controllers to refresh the users.
            FindObjectOfType<TurretController>().addedUser = false;
            FindObjectOfType<BoosterController>().addedUser = false;
            isBlocking = false;
        }
    }
}
