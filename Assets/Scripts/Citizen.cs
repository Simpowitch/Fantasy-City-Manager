using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen
{
    public string Name { get; private set; }
    public bool Female { get; private set; }

    public Citizen()
    {
        Female = Utility.RandomizeBool(50);
        Name = NameGenerator.GetName(Female);
    }
}
