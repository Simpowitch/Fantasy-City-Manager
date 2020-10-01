using UnityEngine;

[System.Serializable]
public class CityResource
{
    public enum Type {Gold, Wood, Stone, Iron, Food }
    public Type type;
    public string ResourceName { get => type.ToString(); }
    public delegate void ResourceHandler(int value);
    public ResourceHandler OnValueChanged;
    [SerializeField] int value;
    public int Value
    {
        get => value;
        set
        {
            this.value = value;
            OnValueChanged?.Invoke(value);
        }
    }
    public CityResource(Type type, int startValue)
    {
        this.type = type;
        this.value = startValue;
        OnValueChanged = null;
    }
}
