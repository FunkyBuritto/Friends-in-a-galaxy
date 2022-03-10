using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityOSC;

public class HoverButton : MonoBehaviour
{
    [SerializeField] private UnityEvent onActivate;
    [SerializeField] private RectTransform hoverArea;
    [SerializeField] private RectTransform mask;
    private Dictionary<GUID, bool> cursors = new Dictionary<GUID, bool>();
    private LobbyMenu menu;
    private bool active;
    private float charge;
    private float maskWidth;

    private void Start()
    {
        OSCHandler.AddGlobalSpecificHook($"/{Constants.UUID}/gravity", LookForHover);
        charge = 0;
        maskWidth = mask.sizeDelta.x;
    }

    private void Update()
    {
        // Handle the timer on hover.
        if (active) charge += Time.deltaTime;
        else charge = Mathf.Clamp(charge - Time.deltaTime / 3.0f, 0.0f, Mathf.Infinity);
        if (charge >= 3.0f) onActivate.Invoke();

        mask.sizeDelta = new Vector2(maskWidth * (1.0f - charge / 3.0f), mask.sizeDelta.y);
    }

    /// <summary>
    /// Check if the cursor is above the button.
    /// </summary>
    /// <param name="msg">The cursor information.</param>
    private void LookForHover(OscMessage msg)
    {
        OSCUser user = LobbyManager.instance.users.Values.First(u => u.ip == msg.ip);
        if (menu == null) menu = FindObjectOfType<LobbyMenu>();
        if (cursors.ContainsKey(user.id) == false) cursors.Add(user.id, false);

        if (menu != null && menu.cursors.Exists(c => c.id == user.id))
        {
            // Get the cursor position.
            Vector3 cursor = menu.cursors.Find(c => c.id == user.id).instance.transform.position;

            // Get the corners of the button:
            Vector3[] corners = new Vector3[4];
            hoverArea.GetWorldCorners(corners);

            // Match the cursor to the button corners:
            cursors[user.id] = cursor.x > corners[0].x && cursor.x < corners[2].x && cursor.y > corners[0].y && cursor.y < corners[1].y;

            // Check if all cursors are over the button:
            active = cursors.Values.All(c => c) && cursors.Values.Count >= 2;
        }
        else if (cursors.ContainsKey(user.id) == true) cursors.Remove(user.id);

        foreach (KeyValuePair<GUID, bool> cursor in cursors) {
            if (!LobbyManager.instance.users.ContainsKey(cursor.Key))
            {
                cursors.Remove(cursor.Key);
                break;
            }
        }
    }
}
