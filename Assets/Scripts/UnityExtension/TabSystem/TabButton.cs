using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TabGroup myTabGroup;
    public GameObject page;
    public TabGroup subTabGroup;
    [HideInInspector] public Image background;
    public TextMeshProUGUI titleText;

    public UnityEvent OnTabButtonSelected;
    public UnityEvent OnTabButtonDeselected;

    public void OnPointerExit(PointerEventData eventData) => myTabGroup.OnTabExit(this);

    public void OnPointerClick(PointerEventData eventData) => myTabGroup.OnTabSelected(this);

    public void OnPointerEnter(PointerEventData eventData) => myTabGroup.OnTabEnter(this);

    private void Start()
    {
        if (myTabGroup == null)
            Debug.LogError("Missing reference to tab group", this.gameObject);
        background = GetComponent<Image>();
        if (myTabGroup)
            myTabGroup.Subscribe(this);
        Deselect();
    }


    public void Select()
    {
        if (page)
            page.SetActive(true);
        OnTabButtonSelected?.Invoke();
    }

    public void Deselect()
    {
        if (page)
            page.SetActive(false);
        OnTabButtonDeselected?.Invoke();
        if (subTabGroup)
            subTabGroup.ResetSelection();
    }
}
