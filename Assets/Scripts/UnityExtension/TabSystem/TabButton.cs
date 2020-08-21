using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TabGroup tabGroup;
    public GameObject page;
    [HideInInspector] public Image background;

    public UnityEvent OnTabSelected;
    public UnityEvent OnTabDeselected;


    public void OnPointerExit(PointerEventData eventData) => tabGroup.OnTabExit(this);

    public void OnPointerClick(PointerEventData eventData) => tabGroup.OnTabSelected(this);

    public void OnPointerEnter(PointerEventData eventData) => tabGroup.OnTabEnter(this);

    private void Start()
    {
        background = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }

    public void Select()
    {
        page.SetActive(true);
        OnTabSelected?.Invoke();
    }

    public void Deselect()
    {
        page.SetActive(false);
        OnTabDeselected?.Invoke();
    }
}
