using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    [Header("Movement")]
    [SerializeField] private float speed = 5f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;

    [Header("Ground Checker")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTimeDuration = 0.15f;

    private const float GROUND_CHECKER_SIZE = 0.025f;
    private Vector3 GOUND_CHECKER_SCALE = new(2.5f, 1f, 2.5f);

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
        HandleRotation();
    }

    private void CheckIfIsOnGround() {
        Vector3 center = transform.position;
        onGround = Physics.CheckBox(center, GOUND_CHECKER_SCALE * GROUND_CHECKER_SIZE, Quaternion.identity, groundLayer, QueryTriggerInteraction.Ignore);
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

    private void HandleRotation() {
        Vector3 direction = new(movementVector.x, 0f, movementVector.y);
        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    private void OnDrawGizmosSelected() {
        Vector3 center = transform.position;
        Gizmos.color = onGround ? Color.green : Color.red;
        Gizmos.DrawWireCube(center, GROUND_CHECKER_SIZE * 2 * GOUND_CHECKER_SCALE);
    }
}