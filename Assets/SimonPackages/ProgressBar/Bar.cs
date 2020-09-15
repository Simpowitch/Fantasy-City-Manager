using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode()]
public class Bar : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Linear Bar")]
    public static void AddLinearBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("Linear Bar"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }
    [MenuItem("GameObject/UI/Radial Bar")]
    public static void AddRadialBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("Radial Bar"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }
#endif

    [SerializeField] int minimumValue;
    [SerializeField] int maximumValue;
    [SerializeField] int currentValue;

    [SerializeField] Image mask = null;
    [SerializeField] Image fill = null;
    [SerializeField] Text currentValueText = null;

    [SerializeField] float animationTime = 0.5f;

    public Color color;

    bool animatingChange;
    Queue<ProgressStatus> changes = new Queue<ProgressStatus>();

    private void Update()
    {
        SetCurrentFill();
        SetColor();
        ShowCurrentValue();
    }

    private void OnEnable()
    {
        if (changes.Count > 0 && !animatingChange)
        {
            ChangeValues(changes.Dequeue());
        }
    }

    void SetColor()
    {
        fill.color = color;
    }

    void SetCurrentFill()
    {
        float currentOffset = currentValue - minimumValue;
        float maximumOffset = maximumValue - minimumValue;
        float fillAmount = currentOffset / maximumOffset;
        mask.fillAmount = fillAmount;
    }

    public void SetNewValues(float percentageFactor) => EnqueueChange(new ProgressStatus(percentageFactor));

    private void EnqueueChange(ProgressStatus change)
    {
        changes.Enqueue(change);
        if (this.gameObject.activeInHierarchy && !animatingChange)
        {
            ChangeValues(changes.Dequeue());
        }
    }

    private void ChangeValues(ProgressStatus newStatus)
    {
        maximumValue = newStatus.newMaximum;
        minimumValue = newStatus.newMinimum;
        StartCoroutine(ChangeCurrentOverTime(newStatus.newCurrent, animationTime));
    }

    IEnumerator ChangeCurrentOverTime(int targetValue, float animationTime)
    {
        animatingChange = true;
        float timer = 0;
        float t = 0;
        int startCurrent = currentValue;

        while (timer < animationTime && currentValue < maximumValue)
        {
            timer += Time.deltaTime;
            t = timer / animationTime;
            currentValue = Mathf.RoundToInt(Mathf.Lerp(startCurrent, targetValue, t));
            yield return null;
        }
        currentValue = targetValue;
        animatingChange = false;

        if (changes.Count > 0)
        {
            ChangeValues(changes.Dequeue());
        }
    }

    private void ShowCurrentValue()
    {
        if (currentValueText != null)
        {
            currentValueText.text = currentValue.ToString() + " / " + maximumValue.ToString();
        }
    }

    public struct ProgressStatus
    {
        public readonly int newCurrent;
        public readonly int newMaximum;
        public readonly int newMinimum;

        public ProgressStatus(float percentageFactor)
        {
            this.newCurrent = Mathf.RoundToInt(percentageFactor * 100);
            this.newMaximum = 100;
            this.newMinimum = 0;
        }
    }
}

