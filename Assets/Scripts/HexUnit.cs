using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class HexUnit : MonoBehaviour
{
    [Header("Movement")]
    const float travelSpeed = 4f;
    List<HexCell> pathToTravel;

    public List<HexCell> ReachableCells { get; private set; } = new List<HexCell>();

    public HexGrid myGrid;

    public bool IsMoving
    {
        get => pathToTravel != null && pathToTravel.Count > 0;
    }

    HexCell location;
    public HexCell Location
    {
        get => location;
        set
        {
            if (location)
            {
                location.UnitsOnCell.Remove(this);
            }
            location = value;
            value.UnitsOnCell.Add(this);
            transform.localPosition = value.Position;
        }
    }

    void OnEnable()
    {
        if (location)
        {
            transform.localPosition = location.Position;
            pathToTravel = null;
        }
    }

    public void ValidateLocation() => transform.localPosition = location.Position;

    public IEnumerator Travel(List<HexCell> path)
    {
        if (!IsMoving && path.Count > 1)
        {
            yield return StartCoroutine(TravelPath());
        }
    }

    IEnumerator TravelPath()
    {
        float zPos = transform.localPosition.z;
        HexCell latestCell = pathToTravel[0];

        Vector3 a, b, c = pathToTravel[0].Position;
        transform.localPosition = c;


        float t = Time.deltaTime * travelSpeed;
        for (int i = 1; i < pathToTravel.Count; i++)
        {
            a = c;
            b = pathToTravel[i - 1].Position;
            c = (b + pathToTravel[i].Position) * 0.5f;

            latestCell = pathToTravel[i - 1];

                for (; t < 1f; t += Time.deltaTime * travelSpeed)
                {
                    //Move
                    Vector3 newPos = Bezier.GetPoint(a, b, c, t);
                    newPos.z = zPos;
                    transform.localPosition = newPos;

                    yield return null;
                }
                t -= 1f;
            Location = pathToTravel[i];
        }

        //Last point
        a = c;
        b = pathToTravel[pathToTravel.Count - 1].Position;
        c = b;

        //Rotation
        latestCell = pathToTravel[pathToTravel.Count - 2];

        for (; t < 1f; t += Time.deltaTime * travelSpeed)
        {
            //Move
            Vector3 newPos = Bezier.GetPoint(a, b, c, t);
            newPos.z = zPos;
            transform.localPosition = newPos;

            yield return null;
        }
        Location = pathToTravel[pathToTravel.Count - 1]; //Set new location
        transform.localPosition = location.Position;
        pathToTravel = null; //Clear the list
    }

    public void Despawn()
    {
        location.UnitsOnCell.Remove(this);
        Destroy(gameObject);
    }
}