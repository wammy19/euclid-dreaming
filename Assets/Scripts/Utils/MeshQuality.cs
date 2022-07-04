#if (UNITY_EDITOR) // Ignore this class from being built. 

using UnityEditor;
using UnityEngine;
using UnityMeshSimplifier;

public class MeshQuality : MonoBehaviour {

    [Range(0.1f, 1f)]
    public float quality = 0.5f;

    /**
     * Reduces the vertex count of a mesh using the Mesh Simplifier Package.
     * GitHub To Package: https://github.com/Unity-Technologies/UnityMeshSimplifier
     * Learning Resources: https://www.youtube.com/watch?v=5y4quM0a_js
     */
    public void ReduceQuality() {

        Mesh originalMesh = GetComponent<MeshFilter>().sharedMesh;
        MeshSimplifier meshSimplifier = new MeshSimplifier();
        
        meshSimplifier.Initialize(originalMesh);
        meshSimplifier.SimplifyMesh(quality);

        Mesh destMesh = meshSimplifier.ToMesh();
        
        SaveMesh(destMesh);
        GetComponent<MeshFilter>().sharedMesh = destMesh;
    }

    private void SaveMesh(Mesh meshToSave) { // Save mesh filter so that we can apply it to a prefab.

        string savePath = "Assets/Models/LowResMeshFilters_" + name + "LowRes.asset";
        Debug.Log("Saved Mesh to:" + savePath); // Indicate where mesh was saved in console.

        AssetDatabase.CreateAsset(meshToSave, savePath); // Create new asset.
    }
}

#endif