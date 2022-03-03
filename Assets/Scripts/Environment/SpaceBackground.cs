using UnityEngine;
using System.Linq;

public class SpaceBackground : MonoBehaviour
{
    [SerializeField] private Background[] backgrounds;

    public static SpaceBackground instance;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Update the background with the given color.
    /// (Make sure to update the inspector of the SpaceBackground script!)
    /// </summary>
    /// <param name="color">The color to match.</param>
    public static void UpdateBackground(Color color)
    {
        Background match = instance.backgrounds.FirstOrDefault(b => new Vector3(Mathf.RoundToInt(color.r * 10.0f), Mathf.RoundToInt(color.g * 10.0f), Mathf.RoundToInt(color.b * 10.0f)) == new Vector3(Mathf.RoundToInt(b.color.r * 10.0f), Mathf.RoundToInt(b.color.g * 10.0f), Mathf.RoundToInt(b.color.b * 10.0f)));
        if (match.obj != null)
        {
            for (int i = 0; i < instance.transform.childCount; i++)
            {
                GameObject child = instance.transform.GetChild(i).gameObject;
                SpriteRenderer sp = child.GetComponent<SpriteRenderer>();

                if (child.name != match.obj.name && !LeanTween.isTweening(child) && child.activeSelf == true)
                {
                    LeanTween.value(child, e => sp.color = new Color(1f, 1f, 1f, e), 1.0f, 0.0f, 2.0f).setOnComplete(() => child.SetActive(false));
                }
                else if (child.name == match.obj.name && !LeanTween.isTweening(child) && sp.color.a != 1.0f)
                {
                    child.SetActive(true);
                    LeanTween.value(child, e => sp.color = new Color(1f, 1f, 1f, e), 0.0f, 1.0f, 2.0f).setOnComplete(() => sp.color = new Color(1.0f, 1.0f, 1.0f, 1.0f));
                }
            }
        }
    }
}

[System.Serializable]
public struct Background
{
    public Color color;
    public GameObject obj;
}