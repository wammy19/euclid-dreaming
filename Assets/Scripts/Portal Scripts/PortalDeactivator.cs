using UnityEngine;

public class PortalDeactivator : MonoBehaviour {
    
    public GameObject portal;

    private void OnTriggerEnter(Collider other) {
        
        portal.SetActive(false);
    }
}
