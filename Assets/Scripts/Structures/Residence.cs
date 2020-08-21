using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Residence : Structure
{
    [Header("Residence")]

    public int maxResidents = 2;
    public List<Citizen> Residents { get; private set; } = new List<Citizen>();
    public int NumberOfUnfilledResidenceSpots { get => maxResidents - Residents.Count; }

    protected override void Constructed(City city, bool addToCityList)
    {
        base.Constructed(city, addToCityList);
        if (addToCityList)
            city.residentialBuildings.Add(this);
        
    }
    public bool IsFunctional() => Residents.Count > 0;

    public bool CanMoveIn() => Residents.Count < maxResidents;
    public void MoveIn(Citizen citizen) => Residents.Add(citizen);
    public void MoveOut(Citizen citizen) => Residents.Remove(citizen);

    public override void Despawn()
    {
        base.city.residentialBuildings.Remove(this);
        foreach (var resident in Residents)
        {
            resident.LeaveHome();
        }
        base.Despawn();
    }
}
