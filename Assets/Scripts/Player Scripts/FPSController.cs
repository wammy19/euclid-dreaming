using System;
using UnityEngine;

public class FPSController : PortalTraveller {

    /*
     * These movement controls are meant to be set from the inspector, but not to be used for calculations.
     * There are to be treated like constants. This is because in other scripts like "AreaScaler.cs",
     * these properties are multiplied by scale factors.
     */
    [Header("Movement Controls:")]
    [Range(5, 30)]
    [SerializeField]
    public float walkSpeed = 15;

    [Range(2, 15)]
    [SerializeField]
    public float jumpForce = 8;

    [Range(0.0f, 1.0f)]
    [SerializeField]
    public float gravityAmount = 0.4f;

    [Header("Mouse Controls:")]
    [Range(50, 200)]
    [SerializeField]
    public float mouseSpeed = 100f;

    [Header("Debug:")]
    public bool drawDebugRays;

    // Properties that can be accessed from other scripts but not the inspector.
    [HideInInspector]
    public float appliedWalkSpeed;

    [HideInInspector]
    public float appliedJumpForce;

    [HideInInspector]
    public float appliedGravityAmount;

    [HideInInspector]
    public GameObject planet;

    [HideInInspector]
    public bool boosting;

    // ---------------------------------------------------------
    // Private Vars.
    private float xRotation;
    private CharacterController controller;
    private Camera playerCamera;
    private Vector3 velocity;
    private Vector3 gravityDir;
    private bool canJump;
    private float previousScale;

    private void Start() {

        playerCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked; // Lock cursor position.
        Cursor.visible = false; // Hide cursor.

        controller = GetComponent<CharacterController>();

        // Init applied movement properties.
        appliedWalkSpeed = walkSpeed;
        appliedGravityAmount = gravityAmount;
        appliedJumpForce = jumpForce;
        boosting = false;
    }

    private void Update() {

        Movement();
        MouseControl();
    }

    private void FixedUpdate() { // Physics calculations.

        HandleJumping();
        JumpRaycast();
    }

    private void Movement() {

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = appliedWalkSpeed * Input.GetAxis("Vertical");
        float curSpeedY = appliedWalkSpeed * Input.GetAxis("Horizontal");

        velocity = (forward * curSpeedX) + (right * curSpeedY);
        controller.Move(velocity * Time.deltaTime);
    }

    /*
     * Handles first person looking rotation for mouse.
     */
    private void MouseControl() {

        float mouseX = Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleJumping() {

        switch (canJump) {

            case true when Input.GetButton("Jump"): // Can jump and pressed jump button.
                gravityDir = appliedJumpForce * transform.up;
                break;

            case true:
                if (!boosting) {
                    gravityDir = Vector3.zero; // Stop applying gravity if we are colliding with the ground.
                }
                break;

            default:
                ApplyGravity(appliedGravityAmount); // If ray cast is not hitting the ground, apply gravity.
                break;
        }

        controller.Move(gravityDir * Time.fixedDeltaTime);
    }

    private void JumpRaycast() {

        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit jumpHitInfo;
        RaycastHit landHitInfo;

        Vector3 localScale = transform.localScale;

        // Multiplier for ray cast length.
        const float jumpRayLenghtMultiplier = 1.5f; // Ray cast length is shorter if the player is shrinking.
        const float landRayLengthMultiplier = 2.5f;

        float jumpRayLength = localScale.x * jumpRayLenghtMultiplier; // Checks if the player is close enough to the ground to jump.
        float landRayLength = localScale.x * landRayLengthMultiplier; // Longer to have smoother walking on slopes and to prevent large jumps in different angles.

        if (drawDebugRays) { // For debugging.

            Debug.DrawRay(transform.position, -transform.up * jumpRayLength, canJump ? Color.green : Color.red);
        }

        // Player can only walk up surfaces that are on layer 8.
        if (Physics.Raycast(ray, out landHitInfo, landRayLength) && landHitInfo.collider.gameObject.layer == 8) {

            // Rotating character based on floor normals.
            Quaternion rotateCharacter = Quaternion.FromToRotation(transform.up, landHitInfo.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, rotateCharacter, Time.fixedDeltaTime * 10f);
        }

        // Sets can jump depending on if the ray is colliding with the ground or not.
        canJump = Physics.Raycast(ray, out jumpHitInfo, jumpRayLength);
    }

    // GravityForceAmount (best if this is a number between 0 and 1).
    private void ApplyGravity(float gravityForceAmount) {
        gravityDir -= (transform.up * gravityForceAmount);
    }

    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 newPosition, Quaternion rotation) {

        transform.localScale = Vector3.one;

        float delta = Mathf.DeltaAngle(0, rotation.eulerAngles.y);

        transform.eulerAngles = Vector3.up * delta;
        velocity = toPortal.TransformVector(fromPortal.InverseTransformVector(velocity));

        appliedGravityAmount = gravityAmount;
        appliedJumpForce = jumpForce;
        appliedWalkSpeed = walkSpeed;

        transform.position = newPosition; // Teleport player to new location.

        Physics.SyncTransforms();
    }

    // Function kinda unnecessary as it uses the same transforms as the teleport function.
    public void RotateChar(Transform fromPortal, Transform toPortal) {

        Vector3 toPortalForward = toPortal.forward;

        // Currently doesn't work completely, rotates character correctly but not view angle.
        Quaternion toRot = Quaternion.FromToRotation(transform.forward, toPortalForward);
        Quaternion rotateCharacter = Quaternion.FromToRotation(fromPortal.forward, toPortalForward) * transform.rotation;

        transform.rotation = rotateCharacter;
    }

    public void BoostPlayer(Vector3 direction, float force) {
        
        gravityDir += direction * force;
        controller.Move(gravityDir * Time.deltaTime);
        boosting = true;
    }
}
