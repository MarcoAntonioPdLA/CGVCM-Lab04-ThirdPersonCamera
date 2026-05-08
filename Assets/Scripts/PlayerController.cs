using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    [Header("Movement")]
    [SerializeField] private float speed = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;

    [Header("Ground Checker")]
    [SerializeField] private LayerMask groundLayer;

    private const float GROUND_CHECKER_RADIUS = 0.05f;

    private Rigidbody rb;
    private Vector2 movementVector;
    private bool onGround = true;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    public void OnMove(InputValue value) {
        movementVector = value.Get<Vector2>();
    }

    public void OnJump() {
        if (onGround) {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate() {
        CheckIfIsOnGround();
        HandleMovement();
    }

    private void CheckIfIsOnGround() {
        Vector3 center = transform.position;
        onGround = Physics.CheckSphere(center, GROUND_CHECKER_RADIUS, groundLayer, QueryTriggerInteraction.Ignore);
    }

    private void HandleMovement() {
        Vector3 direction = new(movementVector.x, 0f, movementVector.y);
        Vector3 newLinearVelocity = direction * speed;
        newLinearVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = newLinearVelocity;
    }

    private void OnDrawGizmosSelected() {
        Vector3 center = transform.position;
        Gizmos.color = onGround ? Color.green : Color.red;
        Gizmos.DrawWireSphere(center, GROUND_CHECKER_RADIUS);
    }
}