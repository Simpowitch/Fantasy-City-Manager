using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasUpkeep
{
    CityResourceGroup Upkeep { get; }
}
