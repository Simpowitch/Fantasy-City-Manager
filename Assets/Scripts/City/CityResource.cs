using UnityEngine;

[System.Serializable]
public class CityResource
{
    public enum Type { Gold, Wood, Stone, Iron, Food }
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
    public Sprite Sprite
    {
        get
        {
            string path = "Sprites/CityResources/";
            switch (type)
            {
                case Type.Gold:
                    break;
                case Type.Wood:
                    path += "Chopped wood";
                    break;
                case Type.Stone:
                    break;
                case Type.Iron:
                    break;
                case Type.Food:
                    break;
                default:
                    return null;
            }
            Sprite sprite = Resources.Load<Sprite>(path);
            if (sprite != null)
                return sprite;
            else
            {
                Debug.LogError($"Sprite not found in path: {path}");
                return null;
            }
        }
    }

    public CityResource(Type type, int startValue)
    {
        this.type = type;
        this.value = startValue;
        OnValueChanged = null;
    }

}
