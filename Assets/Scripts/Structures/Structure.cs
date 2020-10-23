using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Structure : MonoBehaviour, IHasIncome, IHasUpkeep
{
    [Header("Structure")]
    [SerializeField] CityResourceGroup upkeep = new CityResourceGroup();
    [SerializeField] CityResourceGroup income = new CityResourceGroup();

    public District District { get; private set; }

    public virtual void SetDistrict(District district)
    {
        District = district;
    }
    public CityResourceGroup Upkeep => upkeep;
    public CityResourceGroup Income => income;
}
