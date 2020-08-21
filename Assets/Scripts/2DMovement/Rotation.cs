using UnityEngine;

public class Rotation : MonoBehaviour
{
    public void RotateTowards(Vector3 point) => transform.right = point - transform.position;
}
