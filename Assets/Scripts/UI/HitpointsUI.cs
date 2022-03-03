using UnityEngine;

public class HitpointsUI : MonoBehaviour
{
    [SerializeField] private RectTransform fill;
    [SerializeField] private float maxWidth = 750.0f;

    private float maxHp;

    private void Start()
    {
        ShipController player = FindObjectOfType<ShipController>();

        // Setup if the player exists within the scene.
        if (player != null)
        {
            maxHp = player.Hp;
            player.OnHit += OnPlayerHit;
        }
    }

    /// <summary>
    /// Called whenever the player takes a hit.
    /// </summary>
    /// <param name="old_hp">The hitpoints before the player was hit.</param>
    /// <param name="new_hp">The hitpoints after the player was hit.</param>
    private void OnPlayerHit(int old_hp, int new_hp)
    {
        if (LeanTween.isTweening(gameObject)) LeanTween.cancel(gameObject);
        // Animate the bar from the old to the new hitpoints.
        LeanTween.value(gameObject, e => fill.sizeDelta = new Vector2(maxWidth * (e / maxHp), fill.sizeDelta.y), old_hp, new_hp, 0.1f).setDelay(0.1f)
        // Make sure the size is set at the end of the animation!
        .setOnComplete(() => fill.sizeDelta = new Vector2(maxWidth * (new_hp / maxHp), fill.sizeDelta.y));

        // Make the bar jump when hit.
        LeanTween.scale(fill.gameObject, Vector2.one * 0.8f, 0.2f).setEaseInBack()
        .setOnComplete(() => LeanTween.scale(fill.gameObject, Vector2.one, 0.1f));
    }
}
