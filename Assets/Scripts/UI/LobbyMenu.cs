using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityOSC;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private RectTransform usersGrid;
    [SerializeField] private GameObject userUI;
    [SerializeField] private RectTransform cursorParent;
    [SerializeField] private GameObject cursorUI;
    private readonly List<RectTransform> cursors = new List<RectTransform>();
    private readonly List<Vector3> cursorPositions = new List<Vector3>();

    private void Start()
    {
        LobbyManager.instance.OnConnection += OnConnection;
        LobbyManager.instance.OnDisconnect += OnDisconnect;
    }

    private void Update()
    {
        for (int i = 0; i < cursors.Count; i++)
        {
            if (cursors[i] != null)
                cursors[i].position = Vector3.Lerp(cursors[i].position, cursorPositions[i], 0.01f);
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
        obj.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = index switch
        {
            0 => "Driver",
            1 => "Gunner",
            _ => "Dummy",
        };

        // Create a cursor for the user.
        CreateCursor(user, index);
    }

    /// <summary>
    /// Create a new cursor for a user.
    /// </summary>
    private void CreateCursor(OSCUser user, int index)
    {
        // Create the cursor object.
        cursors.Add(Instantiate(cursorUI, cursorParent).GetComponent<RectTransform>());

        // Add a new cursor position to the list.
        cursorPositions.Add(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f));

        // Add the hook.
        user.AddHook("gravity", (msg) => OnCursor(msg, index));
    }

    /// <summary>
    /// Handles the cursors for the individual users.
    /// </summary>
    /// <param name="data">The user movement data</param>
    /// <param name="index">The index of this user</param>
    private void OnCursor(ArrayList data, int index)
    {
        // Check if this cursor still exists.
        if (cursors[index] == null) return;

        float h = -(float)data[0];
        float v = -(float)data[1];

        float width = Screen.width;
        float sh = Mathf.Clamp(width * h + width / 2.0f, 30, Screen.width - 30);

        float height = Screen.height * 1.5f;
        float sv = Mathf.Clamp(height * v + height / 2.0f, 30, Screen.height - 30);

        cursorPositions[index] = new Vector3(sh, sv);
    }

    /// <summary>
    /// Destroys the cursor object for a user.
    /// </summary>
    private void DestroyCursor(int index)
    {
        Destroy(cursors[index].gameObject);
    }

    /// <summary>
    /// Called once a user disconnects from the OSC server.
    /// </summary>
    /// <param name="index">Index of the user within the user list</param>
    private void OnDisconnect(int index)
    {
        DestroyCursor(index);
        Destroy(usersGrid.GetChild(index).gameObject);
    }
}
