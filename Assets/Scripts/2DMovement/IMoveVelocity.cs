using UnityEngine;

public interface IMoveVelocity
{
    void SetVelocity(Vector3 velocityVector);
    void MoveTowards(Vector3 point);
}
