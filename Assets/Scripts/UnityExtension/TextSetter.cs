using UnityEngine;
using UnityEngine.UI;
public class TextSetter : MonoBehaviour
{
    [SerializeField] Text textElement = null;
    public void SetText(string text) => textElement.text = text;
}
