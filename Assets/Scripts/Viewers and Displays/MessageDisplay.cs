using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MessageDisplay : MonoBehaviour
{
    [SerializeField] GameObject chatbubbleObject = null;
    [SerializeField] Text chatbubbleText = null;
    [SerializeField] RectTransform backgroundRectTransform = null;

    [SerializeField] float textPaddingWidth = 1;
    [SerializeField] float textPaddingHeight = 1;
    [SerializeField] int rowMaxWordCount = 6;

    public bool IsShowingMessage { get; private set; }

    public IEnumerator ShowMessage(float timeToShow, string message)
    {
        IsShowingMessage = true;

        UpdateUI(message);
        Vector2 backgroundSize = new Vector2(chatbubbleText.preferredWidth + textPaddingWidth * 2, chatbubbleText.preferredHeight + textPaddingHeight * 2);

        //Rezise to fit text
        backgroundRectTransform.sizeDelta = backgroundSize;

        yield return new WaitForSeconds(timeToShow);
        IsShowingMessage = false;
        UpdateUI("");
    }

    public void ShowMessage(string message)
    {
        UpdateUI(message);
        Vector2 backgroundSize = new Vector2(chatbubbleText.preferredWidth + textPaddingWidth * 2, chatbubbleText.preferredHeight + textPaddingHeight * 2);

        //Rezise to fit text
        backgroundRectTransform.sizeDelta = backgroundSize;
    }

    private void UpdateUI(string message)
    {
        chatbubbleObject.SetActive(message != "");
        if (message != "")
        {
            string[] words = message.Split(' ');
            if (words.Length > rowMaxWordCount)
            {
                message = "";
                for (int i = 0; i < words.Length; i++)
                {
                    message += words[i] + " ";
                    if (i > 0 && i % rowMaxWordCount == 0 && i < words.Length - 2) //Do not make a new row before the last word
                        message += "\n";
                }
            }
            chatbubbleText.text = message;
        }
    }
}
