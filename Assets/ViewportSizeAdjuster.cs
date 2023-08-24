using UnityEngine;
using UnityEngine.UI;

public class ViewportSizeAdjuster : MonoBehaviour
{
    public RectTransform viewport;
    public RectTransform content;
    public float maxHeight;

    void Update()
    {
        float contentHeight = content.rect.height;
        Debug.Log($"content height: {contentHeight}");
        float newHeight = Mathf.Min(contentHeight, maxHeight);
        Debug.Log($"new height: {newHeight}");
        viewport.sizeDelta = new Vector2(viewport.sizeDelta.x, newHeight);
    }
}