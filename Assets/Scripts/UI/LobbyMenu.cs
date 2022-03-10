using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityOSC;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private RectTransform usersGrid;
    [SerializeField] private GameObject userUI;
    [SerializeField] private RectTransform cursorParent;
    [SerializeField] private GameObject cursorUI;
    [HideInInspector] public readonly List<UserCursor> cursors = new List<UserCursor>();

    private void Start()
    {
        LobbyManager.instance.OnConnection += OnConnection;
        LobbyManager.instance.OnDisconnect += OnDisconnect;

        OSCHandler.AddGlobalSpecificHook($"/{Constants.UUID}/gravity", (msg) => OnCursor(msg.values, msg.ip));
    }

    private void Update()
    {
        for (int i = 0; i < cursors.Count; i++)
        {
            if (cursors[i] != null)
                cursors[i].instance.transform.position = Vector3.Lerp(cursors[i].instance.transform.position, cursors[i].pos, 0.01f);
        }
    }

    /// <summary>
    /// Called once a user connects to the OSC server.
    /// </summary>
    /// <param name="addr">The user ip address</param>
    /// <param name="index">The user index within the user list</param>
    private void OnConnection(OSCUser user, int index)
    {
        GameObject obj = Instantiate(userUI, usersGrid);

        // Set the address of the user on the UI.
        obj.transform.Find("Address").GetComponent<TextMeshProUGUI>().text = user.ip;

        // Set the name of the user on the UI:
        obj.transform.Find("Panel").Find("Occupation").GetComponent<TextMeshProUGUI>().text = index switch
        {
            0 => "Driver",
            1 => "Gunner",
            _ => "Spector",
        };

        // Set the color of the panel on the UI:
        Color color;
        switch (index)
        {
            case 0: ColorUtility.TryParseHtmlString("#FF88DC", out color); break;
            case 1: ColorUtility.TryParseHtmlString("#AD88FF", out color); break;
            default: ColorUtility.TryParseHtmlString("#DDDDDD", out color); break;
        }
        obj.transform.Find("Panel").GetComponent<Image>().color = color;

        // Set the color of the text on the UI:
        Color textColor;
        switch (index)
        {
            case 0: ColorUtility.TryParseHtmlString("#FFBDFB", out textColor); break;
            case 1: ColorUtility.TryParseHtmlString("#D5BDFF", out textColor); break;
            default: ColorUtility.TryParseHtmlString("#ECECEC", out textColor); break;
        }
        obj.transform.Find("Panel").Find("Occupation").GetComponent<TextMeshProUGUI>().color = textColor;

        // Create a cursor for the user.
        CreateCursor(user.ip, index);
    }

    /// <summary>
    /// Create a new cursor for a user.
    /// </summary>
    private void CreateCursor(string ip, int index)
    {
        // Get the color of the cursor on the UI:
        Color color;
        switch (index)
        {
            case 0: ColorUtility.TryParseHtmlString("#FF88DC", out color); break;
            case 1: ColorUtility.TryParseHtmlString("#AD88FF", out color); break;
            default: ColorUtility.TryParseHtmlString("#DDDDDD", out color); break;
        }

        GameObject cursor = Instantiate(cursorUI, cursorParent);
        cursor.GetComponent<Image>().color = color;

        // Create the cursor object.
        cursors.Add(new UserCursor(ip, cursor, new Vector3(Screen.width / 2.0f, Screen.height / 2.0f)));
    }

    /// <summary>
    /// Handles the cursors for the individual users.
    /// </summary>
    /// <param name="data">The user movement data</param>
    /// <param name="ip">The ip of this user</param>
    private void OnCursor(ArrayList data, string ip)
    {
        // Check if this cursor still exists.
        if (cursors.Exists(c => c.ip == ip) == false) return;

        float h = -(float)data[0];
        float v = -(float)data[1];

        float width = Screen.width;
        float sh = Mathf.Clamp(width * h + width / 2.0f, 30, Screen.width - 30);

        float height = Screen.height * 1.5f;
        float sv = Mathf.Clamp(height * v + height / 2.0f, 30, Screen.height - 30);

        cursors.Find(c => c.ip == ip).pos = new Vector3(sh, sv);
    }

    /// <summary>
    /// Destroys the cursor object for a user.
    /// </summary>
    private void DestroyCursor(string ip)
    {
        Destroy(cursors.Find(c => c.ip == ip).instance);
        cursors.Remove(cursors.Find(c => c.ip == ip));
    }

    /// <summary>
    /// Called once a user disconnects from the OSC server.
    /// </summary>
    private void OnDisconnect(int index, string ip)
    {
        if (SceneManager.GetActiveScene().name != Constants.GAMESCENE)
        {
            DestroyCursor(ip);
            Destroy(usersGrid.GetChild(index).gameObject);
        }
    }
}
