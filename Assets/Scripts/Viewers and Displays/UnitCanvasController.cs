using UnityEngine;

public class UnitCanvasController : MonoBehaviour
{
    [SerializeField] MessageDisplay interactionDisplay = null;
    [SerializeField] MessageDisplay chatbubbleDisplay = null;
    [SerializeField] Bar progressBar = null;

    public void ShowInteractionText(float timeToShow, string message) => StartCoroutine(interactionDisplay.ShowMessage(timeToShow, message));

    public void ShowChatbubble(float timeToShow, string message) => StartCoroutine(chatbubbleDisplay.ShowMessage(timeToShow, message));

    public void UpdateProgressbar(float factor) => progressBar.SetNewValues(factor);

    public void ShowProgressbar(bool state) => progressBar.gameObject.SetActive(state);
}
