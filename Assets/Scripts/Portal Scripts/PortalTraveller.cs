using UnityEngine;

public abstract class PortalTraveller : MonoBehaviour {

    public Vector3 PreviousOffsetFromPortal { get; set; }

    public abstract void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot);
}
