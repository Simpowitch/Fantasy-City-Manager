using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public delegate void CityHandler(City city);
    public CityHandler OnActiveCityChanged;

    [SerializeField] City defaultCity = null;
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


    private void Start()
    {
        //IF NO LOADING
        CreateNewCity();
        activeCity.cityStats.Setup();
    }

    public void CreateNewCity()
    {
        ActiveCity = defaultCity;
    }
}
