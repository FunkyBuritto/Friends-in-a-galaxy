using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityOSC;

public class HoverButton : MonoBehaviour
{
    [SerializeField] private UnityEvent onActivate;
    [SerializeField] private RectTransform outlineOverlay;
    [SerializeField] private RectTransform mask;
    private readonly Dictionary<string, bool> cursors = new Dictionary<string, bool>();
    private LobbyMenu menu;

    private void Start()
    {
        OSCHandler.AddGlobalSpecificHook($"/{Constants.UUID}/gravity", LookForHover);
    }

    /// <summary>
    /// Check if the cursor is above the button.
    /// </summary>
    /// <param name="msg">The cursor information.</param>
    private void LookForHover(OscMessage msg)
    {
        if (menu == null) menu = FindObjectOfType<LobbyMenu>();
        if (cursors.ContainsKey(msg.ip) == false) cursors.Add(msg.ip, false);

        if (menu.cursors.Exists(c => c.ip == msg.ip))
        {
            // Get the cursor position.
            Vector3 cursor = menu.cursors.Find(c => c.ip == msg.ip).instance.transform.position;

            // Get the corners of the button:
            Vector3[] corners = new Vector3[4];
            mask.GetWorldCorners(corners);

            // Match the cursor to the button corners:
            cursors[msg.ip] = cursor.x > corners[0].x && cursor.x < corners[2].x && cursor.y > corners[0].y && cursor.y < corners[1].y;

            // Check if all cursors are over the button:
            if (cursors.Values.All(c => c))
            {
                if (onActivate != null) onActivate.Invoke();
                mask.GetComponent<Image>().color = Color.blue;
            }
            else mask.GetComponent<Image>().color = Color.red;
        }
        else if (cursors.ContainsKey(msg.ip) == true) cursors.Remove(msg.ip);
    }
}
