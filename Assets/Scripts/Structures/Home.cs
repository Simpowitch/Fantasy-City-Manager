using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Home : Structure
{
    public int capacity;
    public Citizen[] Citizens { get; private set; }
    public int NumberOfCitizens 
    { 
        get
        {
            int numbers = 0;
            for (int i = 0; i < Citizens.Length; i++)
            {
                if (Citizens[i] != null)
                    numbers++;
            }
            return numbers;
        }
    }
    public int FreeSpaces { get => Citizens.Length - NumberOfCitizens; }

    private void Awake()
    {
        Citizens = new Citizen[capacity];
    }

    public override void SetDistrict(District district)
    {
        base.SetDistrict(district);
        district.PopulationCapacity += capacity;
    }
}
