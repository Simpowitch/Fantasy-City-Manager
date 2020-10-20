using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public delegate void CityHandler(City city);
    public CityHandler OnActiveCityChanged;

    private City activeCity;
    public City ActiveCity
    {
        get => activeCity;
        set
        {
            activeCity = value;
            OnActiveCityChanged?.Invoke(value);
        }
    }

    public Transform playerCharacter;
}
