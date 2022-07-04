using UnityEngine;

public class HubWorldPlayerTracker : MonoBehaviour {

    public GameObject player;

    private BoxCollider boxCollider;
    private CharacterController playerCharacterController;
    private float boxSize;

    private void Awake() {

        boxCollider = gameObject.GetComponent<BoxCollider>();
        playerCharacterController = player.GetComponent<CharacterController>();
        boxSize = boxCollider.size.x * 0.5f; // Minus 2 seems to make wrapping transition smoother.
    }

    private void OnTriggerExit(Collider other) {

        Vector3 playerPosition = player.transform.position;


        if (Vector3.Distance(player.transform.position, transform.position) > boxSize * 3) return;

        if (playerPosition.y < transform.position.y - boxSize) {

            playerCharacterController.enabled = false; // Character controller must be disabled before changing the transform.position.
            player.transform.position = new Vector3(playerPosition.x, transform.position.y + boxSize - 2, playerPosition.z); // 2 units if the player is vertical.
            playerCharacterController.enabled = true;
        }
        else if (playerPosition.y > transform.position.y + boxSize) {

            playerCharacterController.enabled = false; // Character controller must be disabled before changing the transform.position.
            player.transform.position = new Vector3(playerPosition.x, transform.position.y - boxSize - 2, playerPosition.z); // 2 units if the player is vertical.
            playerCharacterController.enabled = true;
        }

        if (playerPosition.x < transform.position.x - boxSize) {

            playerCharacterController.enabled = false;
            player.transform.position = new Vector3(transform.position.x + boxSize - 1, playerPosition.y, playerPosition.z); // 1 unit padding if the player is horizontal.
            playerCharacterController.enabled = true;
        }
        else if (playerPosition.x > transform.position.x + boxSize) {

            playerCharacterController.enabled = false;
            player.transform.position = new Vector3(transform.position.x - boxSize + 1, playerPosition.y, playerPosition.z);
            playerCharacterController.enabled = true;
        }

        if (playerPosition.z < transform.position.z - boxSize) {

            playerCharacterController.enabled = false;
            player.transform.position = new Vector3(playerPosition.x, playerPosition.y, transform.position.z + boxSize - 1);
            playerCharacterController.enabled = true;
        }
        else if (playerPosition.z > transform.position.z + boxSize) {

            playerCharacterController.enabled = false;
            player.transform.position = new Vector3(playerPosition.x, playerPosition.y, transform.position.z - boxSize + 1);
            playerCharacterController.enabled = true;
        }
    }
}
