using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IndexUi : MonoBehaviour
{
    public TextMeshProUGUI keyValueText;
    public Image background;

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = transform as RectTransform;
    }

    public void SetColor(Color color)
    {
        if (background != null)
        {
            background.color = color;
        }
    }

    public void SetText(string text)
    {
        if (keyValueText != null)
        {
            keyValueText.text = text;
        }
    }

    public void SetSize(float width, float height)
    {
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(width, height);
        }
    }
}
