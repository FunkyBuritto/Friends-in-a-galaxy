using System.Collections;
using TMPro;
using UnityEngine;
using UnityOSC;

public class Overlay : MonoBehaviour
{
    [SerializeField] private GameObject overlay;
    [SerializeField] private GameObject intro_text;
    public static bool isBlocking = false;

    private void Start()
    {
        StartCoroutine(nameof(IntroSequence));
        //LobbyManager.instance.OnDisconnect += (_, __) => OnDisconnect();
        //LobbyManager.instance.OnConnection += OnConnect;
    }

    IEnumerator IntroSequence()
    {
        TextMeshProUGUI text = intro_text.GetComponent<TextMeshProUGUI>();
        intro_text.SetActive(true);
        text.alpha = 0.9f;
        yield return new WaitForSeconds(5.0f);
        LeanTween.value(gameObject, e => text.alpha = e, 0.9f, 0.0f, 1.0f);
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
            StartCoroutine(nameof(IntroSequence));
            // Force the controllers to refresh the users.
            FindObjectOfType<TurretController>().addedUser = false;
            FindObjectOfType<BoosterController>().addedUser = false;
            isBlocking = false;
        }
    }
}
