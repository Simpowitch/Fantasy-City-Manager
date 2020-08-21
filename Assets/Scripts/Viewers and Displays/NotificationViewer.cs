using UnityEngine;
using UnityEngine.UI;

public class NotificationViewer : MonoBehaviour
{
    [SerializeField] Animator animator = null;
    [SerializeField] Text title = null;
    [SerializeField] Text message = null;

    public void ShowMessage(string title, string message)
    {
        animator.SetTrigger("Show");
        this.title.text = title;
        this.message.text = message;
    }
}
