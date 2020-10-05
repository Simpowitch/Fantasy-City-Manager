using UnityEngine;

public class UnitCanvasController : MonoBehaviour
{
    [SerializeField] MessageDisplay nameTag = null;
    [SerializeField] MessageDisplay chatbubbleDisplay = null;
    [SerializeField] Bar progressBar = null;

    public void ShowNameTag(string message) => nameTag.ShowMessage(message);

    public void ShowChatbubble(float timeToShow, string message) => StartCoroutine(chatbubbleDisplay.ShowMessage(timeToShow, message));

    public void UpdateProgressbar(float factor) => progressBar.SetNewValues(factor);

    public void ShowProgressbar(bool state) => progressBar.gameObject.SetActive(state);
}
