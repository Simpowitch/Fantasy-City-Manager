using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MessageDisplay : MonoBehaviour
{
    public enum MessageType { Interaction, Chatbubble}
    [SerializeField] GameObject interactionObject = null;
    [SerializeField] Text interactionText = null;
    [SerializeField] GameObject chatbubbleObject = null;
    [SerializeField] Text chatbubbleText = null;
    [SerializeField] RectTransform[] backgroundRectTransforms = null;

    float textPaddingWidth = 1;
    float textPaddingHeight = 1;

    public bool IsShowingMessage { get; private set; }

    public IEnumerator ShowMessage(float timeToShow, string message, MessageType messageType)
    {
        IsShowingMessage = true;
        Vector2 backgroundSize = Vector2.zero;

        switch (messageType)
        {
            case MessageType.Interaction:
                ShowInteraction(message, true);
                backgroundSize = new Vector2(interactionText.preferredWidth + textPaddingWidth * 2, interactionText.preferredHeight + textPaddingHeight * 2);
                break;
            case MessageType.Chatbubble:
                ShowChatbubble(message, true);
                backgroundSize = new Vector2(chatbubbleText.preferredWidth + textPaddingWidth * 2, chatbubbleText.preferredHeight + textPaddingHeight * 2);
                break;
        }

        //Rezise to fit text
        foreach (var item in backgroundRectTransforms)
        {
            item.sizeDelta = backgroundSize;
        }

        yield return new WaitForSeconds(timeToShow);
        IsShowingMessage = false;
        switch (messageType)
        {
            case MessageType.Interaction:
                ShowInteraction("", false);
                break;
            case MessageType.Chatbubble:
                ShowChatbubble("", false);
                break;
        }
    }

    private void ShowInteraction(string message, bool status)
    {
        interactionObject.SetActive(status);
        interactionText.text = message;
    }

    private void ShowChatbubble(string message, bool status)
    {
        chatbubbleObject.SetActive(status);
        chatbubbleText.text = message;
    }
}
