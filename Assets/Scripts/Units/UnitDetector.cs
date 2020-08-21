using System.Collections.Generic;
using UnityEngine;

public class UnitDetector : MonoBehaviour
{
    public List<Unit> SearchForUnits(Unit searcher, float radius)
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(searcher.transform.position, radius);
        List<Unit> otherUnits = new List<Unit>();
        foreach (var item in collisions)
        {
            Unit unit = item.GetComponent<Unit>();
            if (unit && unit != searcher)
            {
                otherUnits.Add(unit);
            }
        }
        return otherUnits;
    }
}
