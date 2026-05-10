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

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTimeDuration = 0.15f;

    private const float GROUND_CHECKER_SIZE = 0.025f;

    private Rigidbody rb;
    private Vector2 movementVector;
    private float coyoteTimeCounter = 0f;
    private bool jumpedThisFrame = false;
    private bool onGround = true;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    public void OnMove(InputValue value) {
        movementVector = value.Get<Vector2>();
    }

    public void OnJump() {
        if (coyoteTimeCounter > 0f) {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            coyoteTimeCounter = 0f;
            jumpedThisFrame = true;
        }
    }

    private void FixedUpdate() {
        CheckIfIsOnGround();
        HandleCoyoteTime();
        HandleMovement();
    }

    private void CheckIfIsOnGround() {
        Vector3 center = transform.position;
        onGround = Physics.CheckBox(center, new Vector3(2, 1, 2) * GROUND_CHECKER_SIZE, Quaternion.identity, groundLayer, QueryTriggerInteraction.Ignore);
    }

    private void HandleCoyoteTime() {
        if (onGround && !jumpedThisFrame) {
            coyoteTimeCounter = coyoteTimeDuration;
        }
        else {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }
        jumpedThisFrame = false;
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
        Gizmos.DrawWireCube(center, GROUND_CHECKER_SIZE * 2 * new Vector3(2, 1, 2));
    }
}