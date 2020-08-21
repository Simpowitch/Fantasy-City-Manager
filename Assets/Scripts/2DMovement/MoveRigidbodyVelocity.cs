using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MoveRigidbodyVelocity : MonoBehaviour, IMoveVelocity
{
    [SerializeField] float movementSpeed = 1;

    private Vector3 velocityVector;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetVelocity(Vector3 velocityVector) => this.velocityVector = velocityVector.normalized;

    public void MoveTowards(Vector3 point) => SetVelocity(point - this.transform.position);

    private void FixedUpdate()
    {
        rb.velocity = velocityVector * movementSpeed;
    }
}
