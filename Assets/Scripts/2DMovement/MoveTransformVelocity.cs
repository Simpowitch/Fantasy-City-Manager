using UnityEngine;

public class MoveTransformVelocity : MonoBehaviour, IMoveVelocity
{
    [SerializeField] float movementSpeed = 1;

    private Vector3 velocityVector;

    public void SetVelocity(Vector3 velocityVector) => this.velocityVector = velocityVector.normalized;

    public void MoveTowards(Vector3 point) => SetVelocity(point - this.transform.position);

    private void Update()
    {
        transform.position += velocityVector * movementSpeed * Time.deltaTime;
    }
}
