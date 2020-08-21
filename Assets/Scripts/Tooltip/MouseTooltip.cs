using UnityEngine;
using UnityEngine.UI;

public class MouseTooltip : MonoBehaviour
{
    public enum ColorText { Default, Allowed, Forbidden }


    [Header("References")]
    [SerializeField] RectTransform canvasRect = null;
    [SerializeField] RectTransform mainRectTransform = null;
    [SerializeField] Text textField = null;
    [SerializeField] RectTransform backgroundRect = null;
    [SerializeField] Animator animator = null;

    [Header("Colors")]
    [SerializeField] Color defaultColor = Color.white;
    [SerializeField] Color allowed = Color.green;
    [SerializeField] Color forbidden = Color.red;

    float textPadding = 0;

    bool hidden = true;

    private void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, null, out Vector2 localPoint);
        transform.localPosition = localPoint;

        Vector2 anchoredPosition = mainRectTransform.anchoredPosition;
        if (anchoredPosition.x + backgroundRect.rect.width > canvasRect.rect.width)
        {
            anchoredPosition.x = canvasRect.rect.width - backgroundRect.rect.width;
        }
        if (anchoredPosition.y + backgroundRect.rect.height > canvasRect.rect.height)
        {
            anchoredPosition.y = canvasRect.rect.height - backgroundRect.rect.height;
        }
        mainRectTransform.anchoredPosition = anchoredPosition;
    }

    private void Start()
    {
        textPadding = textField.rectTransform.anchoredPosition.x;
        Hide();
    }

    public void SetUp(ColorText textColor, string message)
    {
        if (message == "")
        {
            Hide();
            return;
        }

        transform.SetAsLastSibling();

        if (hidden)
            animator.SetTrigger("FadeIn");

        hidden = false;

        Color color = defaultColor;
        switch (textColor)
        {
            case ColorText.Default:
                color = defaultColor;
                break;
            case ColorText.Allowed:
                color = allowed;
                break;
            case ColorText.Forbidden:
                color = forbidden;
                break;
        }

        textField.color = color;
        textField.text = message;

        Vector2 backgroundSize = new Vector2(textField.preferredWidth + textPadding * 2, textField.preferredHeight + textPadding * 2);
        backgroundRect.sizeDelta = backgroundSize;
    }

    public void Hide()
    {
        if (!hidden)
            animator.SetTrigger("FadeOut");
        hidden = true;
    }
}
