using UnityEngine;
using UnityEngine.UI;
using UnityOSC;

public class HoverButton : MonoBehaviour
{
    [SerializeField] private RectTransform outlineOverlay;
    [SerializeField] private RectTransform mask;

    private void Start()
    {
        OSCHandler.AddGlobalSpecificHook("/Device/gravity", LookForHover);
    }

    private void LookForHover(OscMessage msg)
    {
        if (FindObjectOfType<LobbyMenu>().cursorPositions[LobbyManager.instance.users.FindIndex(u => u.ip == msg.ip)] != null)
        {
            float x = FindObjectOfType<LobbyMenu>().cursorPositions[LobbyManager.instance.users.FindIndex(u => u.ip == msg.ip)].x;
            float y = FindObjectOfType<LobbyMenu>().cursorPositions[LobbyManager.instance.users.FindIndex(u => u.ip == msg.ip)].y;

            //Debug.Log("pos:" + sh + "," + sv);
            //Debug.Log(mask.rect.xMax);

            //Debug.Log("left: " + mask.rect.xMin + " | x: " + (sh - Screen.width / 2.0f));
            //Debug.Log("top: " + mask.rect.top + " | y: " + v);

            Debug.Log("left: " + (mask.position.x - mask.sizeDelta.x) + " | x: " + x);

            if (x < mask.position.x + mask.sizeDelta.x / 2.0f && x > mask.position.x - mask.sizeDelta.x / 2.0f && y > mask.position.y - mask.sizeDelta.y / 2.0f && y < mask.position.y + mask.sizeDelta.y / 2.0f)
            {
                mask.GetComponent<Image>().color = Color.blue;
                Debug.Log("Hovering!");
            }
            else
            {
                mask.GetComponent<Image>().color = Color.red;
            }
        }
    }
}
