using UnityEngine;

public class PortalActivator : MonoBehaviour {

    public GameObject portal;

    private void OnTriggerEnter(Collider other) {
        
        portal.SetActive(true);
    }
}
