using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Structure : MonoBehaviour
{
    [Header("Structure")]
    public int dailyUpkeep = 1;

    public District District { get; private set; }

    public virtual void SetDistrict(District district)
    {
        District = district;
    }
}
