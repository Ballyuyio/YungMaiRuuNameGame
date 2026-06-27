using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private Rigidbody rb;
    private Vector3 movements;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Prevent the player from tipping over
        rb.freezeRotation = true;
    }

    void Update()
    {
        // 1. Gather input in Update() so no key presses are missed
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Calculate the isometric direction vector
        movements = new Vector3(horizontal + vertical, 0f, vertical - horizontal).normalized;
    }

    void FixedUpdate()
    {
        // 1. Move using Velocity instead of MovePosition.
        // We multiply the movement vector by speed, but KEEP the current Y velocity so gravity still works!
        Vector3 targetVelocity = movements * moveSpeed;
        rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);

        // 2. Face the movement direction smoothly
        if (movements != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movements);
            rb.MoveRotation(Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            ));
        }
    }
}