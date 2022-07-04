using UnityEngine;

public class AreaScaler : MonoBehaviour {

    [Header("Main Properties")]
    public float boundingSphereRadius = 10;
    public float desiredScaleSize = 1; // Don't make zero as this will result in the player scaling down to that size which is impossible.

    [Header("Debug")]
    public bool showDebugLines;
    
    private FPSController playerController;
    private SphereCollider sphereCollider;
    private GameObject player;

    private void Awake() {

        // Add and init sphere collider component.
        sphereCollider = gameObject.AddComponent(typeof(SphereCollider)) as SphereCollider;

        if (sphereCollider is null) return;
        
        sphereCollider.isTrigger = true;
        sphereCollider.radius = boundingSphereRadius;
    }

    private void Start() {

        // Init player components.
        player = GameObject.Find("Player");
        playerController = player.GetComponent<FPSController>();
    }

    // Player enters the sphere collider.
    private void OnTriggerStay(Collider other) {

        // Grab two points to calculate the distance.
        Vector3 playerPosition = player.transform.position;
        Vector3 sphereCenterPoint = transform.position;

        float distance = Vector3.Distance(playerPosition, sphereCenterPoint);

        if (showDebugLines) { // Display debug aid.
            DrawDebugLineBetweenPoints(playerPosition, sphereCenterPoint, distance);
        }

        float size = sphereCollider.radius;
        float newScale = Map(distance, size, 0, 1f, desiredScaleSize);

        ScalePlayerProperties(newScale);
    }

    private void OnTriggerExit(Collider other) {
        
        ResetPlayerProperties(other);
    }

    private void ScalePlayerProperties(float newScale) {

        player.transform.localScale = new Vector3(newScale, newScale, newScale);
        
        // Apply new scale to various player properties.
        playerController.appliedWalkSpeed = playerController.walkSpeed * newScale;
        playerController.appliedJumpForce = playerController.jumpForce * newScale;
        playerController.appliedGravityAmount = playerController.gravityAmount * newScale;

        Physics.SyncTransforms();
    }

    // Resets all player properties when exiting a scaler area.
    private void ResetPlayerProperties(Collider other) {
        
        other.transform.localScale = new Vector3(1, 1, 1);
        playerController.appliedWalkSpeed = playerController.walkSpeed;
        playerController.appliedJumpForce = playerController.jumpForce;
        playerController.appliedGravityAmount = playerController.gravityAmount;
        
        Physics.SyncTransforms();
    }

    /*
     * Draws a red line between two points passed in as well as logs the distance between the two points.
     */
    private void DrawDebugLineBetweenPoints(Vector3 pointA, Vector3 pointB, float distance) {

        Debug.DrawLine(pointA, pointB, Color.red);
        Debug.Log("Distance from player to area point: " + distance);
    }

    /**
     * Learning resources:
     * Formula for remapping numbers: https://stackoverflow.com/questions/3451553/value-remapping
     */
    private static float Map(float value, float from1, float to1, float from2, float to2) {

        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
