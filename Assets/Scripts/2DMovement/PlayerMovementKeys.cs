using UnityEngine;

public class PlayerMovementKeys : MonoBehaviour
{
    MoveTransformVelocity moveVelocity;
    Rotation rotation;

    private void Awake()
    {
        moveVelocity = GetComponent<MoveTransformVelocity>();
        rotation = GetComponent<Rotation>();
    }

    private void Update()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) moveY += 1f;
        if (Input.GetKey(KeyCode.A)) moveX -= 1f;
        if (Input.GetKey(KeyCode.S)) moveY -= 1f;
        if (Input.GetKey(KeyCode.D)) moveX += 1f;

        Vector3 moveVector = new Vector3(moveX, moveY).normalized;

        moveVelocity.SetVelocity(moveVector);
        rotation.RotateTowards(this.transform.position + moveVector);
    }
}
