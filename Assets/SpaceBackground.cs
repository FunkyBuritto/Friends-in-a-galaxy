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
                if (instance.transform.GetChild(i).name != match.obj.name && !LeanTween.isTweening(instance.transform.GetChild(i).gameObject))
                {
                    int j = i;
                    LeanTween.alpha(instance.transform.GetChild(j).gameObject, 0.0f, 2.0f).setOnComplete(() => instance.transform.GetChild(j).gameObject.SetActive(false));
                }
                else if (!LeanTween.isTweening(instance.transform.GetChild(i).gameObject))
                {
                    instance.transform.GetChild(i).gameObject.SetActive(true);
                    LeanTween.alpha(instance.transform.GetChild(i).gameObject, 1.0f, 2.0f);
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