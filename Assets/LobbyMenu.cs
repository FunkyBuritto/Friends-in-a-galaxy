using TMPro;
using UnityEngine;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private RectTransform usersGrid;
    [SerializeField] private GameObject userUI;

    private void Start()
    {
        LobbyManager.instance.OnConnection += OnConnection;
        LobbyManager.instance.OnDisconnect += OnDisconnect;
    }

    /// <summary>
    /// Called once a user connects to the OSC server.
    /// </summary>
    /// <param name="addr">The user ip address</param>
    /// <param name="index">The user index within the user list</param>
    private void OnConnection(string addr, int index)
    {
        GameObject obj = Instantiate(userUI, usersGrid);

        // Set the address of the user on the UI.
        obj.transform.Find("Address").GetComponent<TextMeshProUGUI>().text = addr;

        // Set the name of the user on the UI:
        obj.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = index switch
        {
            0 => "Driver",
            1 => "Gunner",
            _ => "Dummy",
        };
    }

    /// <summary>
    /// Called once a user disconnects from the OSC server.
    /// </summary>
    /// <param name="index">Index of the user within the user list</param>
    private void OnDisconnect(int index)
    {
        Destroy(usersGrid.GetChild(index).gameObject);
    }
}
