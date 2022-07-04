using UnityEngine;
using UnityEngine.Rendering;

public class MainCamera : MonoBehaviour {

    private Portal[] portals;

    private void Awake() {

        portals = FindObjectsOfType<Portal>();
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
    }
    
    /**
     * On PreCull() doesn't work with URP. Use this instead.
     * 
     * Learning Resources:
     * Unity Docs on URP Rendering: https://docs.unity3d.com/ScriptReference/Rendering.RenderPipelineManager-beginCameraRendering.html
     */
    private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera) {
        
        foreach (Portal portal in portals) {
            portal.Render(context, camera);
        }
    }

    // Removes functions attached to the render pipeline. (MUST BE CALLED). 
    private void OnDestroy() {
        
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    }
}
