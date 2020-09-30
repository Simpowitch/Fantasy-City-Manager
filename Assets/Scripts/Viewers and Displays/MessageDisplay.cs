using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MessageDisplay : MonoBehaviour
{
    [SerializeField] GameObject chatbubbleObject = null;
    [SerializeField] Text chatbubbleText = null;
    [SerializeField] RectTransform backgroundRectTransform = null;

    float textPaddingWidth = 1;
    float textPaddingHeight = 1;

    public bool IsShowingMessage { get; private set; }

    public IEnumerator ShowMessage(float timeToShow, string message)
    {
        IsShowingMessage = true;
        Vector2 backgroundSize = Vector2.zero;

        UpdateUI(message, true);
        backgroundSize = new Vector2(chatbubbleText.preferredWidth + textPaddingWidth * 2, chatbubbleText.preferredHeight + textPaddingHeight * 2);

        //Rezise to fit text
        backgroundRectTransform.sizeDelta = backgroundSize;

        yield return new WaitForSeconds(timeToShow);
        IsShowingMessage = false;
        UpdateUI("", false);
    }


    private void UpdateUI(string message, bool status)
    {
        chatbubbleObject.SetActive(status);
        chatbubbleText.text = message;
    }
}
