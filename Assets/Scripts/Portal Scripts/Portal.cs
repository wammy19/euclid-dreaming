using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

public class Portal : MonoBehaviour {

    [Header("Main Settings")]
    public Portal linkedPortal;
    public MeshRenderer screen;
    public int recursionLimit = 5;

    [Header("Advanced Settings")]
    public float nearClipOffset = 0.05f;
    public float nearClipLimit = 0.2f;

    // Private variables
    private RenderTexture viewTexture;
    private Camera portalCam;
    private Camera playerCam;
    private Material firstRecursionMat;
    private List<PortalTraveller> trackedTravellers;
    private MeshFilter screenMeshFilter;
    private Vector3[] renderPositions;
    private Quaternion[] renderRotations;

    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private static readonly int DisplayMask = Shader.PropertyToID("displayMask");

    private void Awake() {
        
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera>();
        trackedTravellers = new List<PortalTraveller>();
        screenMeshFilter = screen.GetComponent<MeshFilter>();
        screen.material.SetInt(DisplayMask, 1);
        portalCam.enabled = false;
        
        RenderPipeline.beginCameraRendering += Render; // Add render function to URP.
        
        renderPositions = new Vector3[recursionLimit];
        renderRotations = new Quaternion[recursionLimit];
    }

    private void LateUpdate() {
        
        HandleTravellers();
    }

    private void HandleTravellers() {
        
        for (int i = 0; i < trackedTravellers.Count; i++) {

            PortalTraveller traveller = trackedTravellers[i];
            Transform travellerT = traveller.transform;
            Matrix4x4 m = linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * travellerT.localToWorldMatrix;

            Vector3 offsetFromPortal = travellerT.position - transform.position;
            int portalSide = Math.Sign(Vector3.Dot(offsetFromPortal, transform.forward));
            int portalSideOld = Math.Sign(Vector3.Dot(traveller.PreviousOffsetFromPortal, transform.forward));
            
            if (portalSide == portalSideOld) continue;

            // Teleport the traveller if it has crossed from one side of the portal to the other.
            traveller.Teleport(transform, linkedPortal.transform, m.GetColumn(3), m.rotation);
            
            // Can't only rely on OnTriggerEnter/Exit to be called next frame since it depends on when FixedUpdate runs.
            linkedPortal.OnTravellerEnterPortal(traveller);
            trackedTravellers.RemoveAt(i);
            i--;
        }
    }

    // Manually render the camera attached to this portal.
    public void Render(ScriptableRenderContext context, Camera camera) {

        // Skip rendering the view from this portal if player is not looking at the linked portal.
        if (!CameraUtility.VisibleFromCamera(linkedPortal.screen, playerCam)) return;

        CreateViewTexture();

        Matrix4x4 localToWorldMatrix = playerCam.transform.localToWorldMatrix;
        
        int startIndex = 0;
        
        portalCam.projectionMatrix = playerCam.projectionMatrix;

        for (int i = 0; i < recursionLimit; i++) {
        
            if (i > 0) {
                
                // No need for recursive rendering if linked portal is not visible through this portal.
                if (!CameraUtility.BoundsOverlap(screenMeshFilter, linkedPortal.screenMeshFilter, portalCam)) {
                    break;
                }
            }
        
            localToWorldMatrix = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * localToWorldMatrix;
            int renderOrderIndex = recursionLimit - i - 1;
            renderPositions[renderOrderIndex] = localToWorldMatrix.GetColumn(3);
            renderRotations[renderOrderIndex] = localToWorldMatrix.rotation;
        
            portalCam.transform.SetPositionAndRotation(renderPositions[renderOrderIndex], renderRotations[renderOrderIndex]);
            startIndex = renderOrderIndex;
        }

        // Hide screen so that camera can see through portal.
        screen.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        linkedPortal.screen.material.SetInt(DisplayMask, 0);

        for (int i = startIndex; i < recursionLimit; i++) {

            portalCam.transform.SetPositionAndRotation(renderPositions[i], renderRotations[i]);
            
            SetNearClipPlane();
            HandleClipping();
            
            // Issue - This RenderSingleCamera() call is causing a big drop in frame rate. Already to deep into the URP to switch back to old pipeline now.
            // There doesn't seem to be a resolve for this issue at the time of writing.
            // https://forum.unity.com/threads/urp-massively-slower-than-built-in-when-doing-recursive-portal-rendering.868405/ 
            UniversalRenderPipeline.RenderSingleCamera(context, portalCam); // Manually render camera.

            if (i == startIndex) {
                linkedPortal.screen.material.SetInt(DisplayMask, 1);
            }
        }

        // Un-hide objects hidden at start of render.
        screen.shadowCastingMode = ShadowCastingMode.On;
    }

    private void HandleClipping() {

        float halfHeight = playerCam.nearClipPlane * Mathf.Tan(playerCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * playerCam.aspect;
        float destToNearClipPlaneCorner = new Vector3(halfWidth, halfHeight, playerCam.nearClipPlane).magnitude;

        Transform screenT = screen.transform;
        bool camFacingSameDirectionAsPortal = Vector3.Dot(transform.forward, transform.position - playerCam.transform.position) > 0;
        
        screenT.localScale = new Vector3(screenT.localScale.x, screenT.localScale.y, destToNearClipPlaneCorner);
        screenT.localPosition = Vector3.forward * destToNearClipPlaneCorner * ((camFacingSameDirectionAsPortal) ? 0.5f : -0.5f);
    }

    private void CreateViewTexture() {

        if (viewTexture != null && viewTexture.width == Screen.width && viewTexture.height == Screen.height) return;
        
        if (viewTexture != null) {
            viewTexture.Release();
        }

        viewTexture = new RenderTexture(Screen.width, Screen.height, 0);

        // Render the view from the portal camera to the view texture.
        portalCam.targetTexture = viewTexture;
        
        // Display the view texture on the screen of the linked portal.
        linkedPortal.screen.material.SetTexture(MainTex, viewTexture);
    }

    // Use custom projection matrix to align portal camera's near clip plane with the surface of the portal.
    // Note that this affects precision of the depth buffer, which can cause issues with effects like screenspace AO.
    private void SetNearClipPlane() { 
        
        Transform clipPlane = transform;
        int dot = Math.Sign(Vector3.Dot(clipPlane.forward, transform.position - portalCam.transform.position));

        Vector3 camSpacePos = portalCam.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        Vector3 camSpaceNormal = portalCam.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
        float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal) + nearClipOffset;

        // Don't use oblique clip plane if very close to portal as it seems this can cause some visual artifacts.
        if (Mathf.Abs(camSpaceDst) > nearClipLimit) {
        
            Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);
        
            // Update projection based on new clip plane.
            // Calculate matrix with player cam so that player camera settings (fov, etc) are used.
            portalCam.projectionMatrix = playerCam.CalculateObliqueMatrix(clipPlaneCameraSpace);
        
            return;
        }
        
        portalCam.projectionMatrix = playerCam.projectionMatrix;
    }

    private void OnTravellerEnterPortal(PortalTraveller traveller) {

        if (trackedTravellers.Contains(traveller)) return;
        
        traveller.PreviousOffsetFromPortal = traveller.transform.position - transform.position;
        
        trackedTravellers.Add(traveller);
        
        Physics.SyncTransforms();
    }

    private void OnTriggerEnter(Collider other) {

        PortalTraveller traveller = other.GetComponent<PortalTraveller>();

        if (traveller) {
            OnTravellerEnterPortal(traveller);
        }
    }

    private void OnTriggerExit(Collider other) {

        PortalTraveller traveller = other.GetComponent<PortalTraveller>();

        if (!traveller || !trackedTravellers.Contains(traveller)) return;
        
        trackedTravellers.Remove(traveller);
    }
    
    // Destroy Render Pipeline functions when game stops playing. This is mostly not to break the editor.
    private void OnDestroy() {
        
        RenderPipelineManager.beginCameraRendering -= Render;
    }
    
    private void OnValidate() {

        if (linkedPortal != null) {
            linkedPortal.linkedPortal = this;
        }
    }
}
