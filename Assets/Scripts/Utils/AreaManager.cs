using UnityEngine;
using UnityEngine.SocialPlatforms;

/*
 * This class manages the hub world area by duplicating the main
 * area a number of times in ever direction to give the illusion of being in an infinite space.
 */
public class AreaManager : MonoBehaviour {

    public GameObject hubWorldMainArea;
    public GameObject hubBoxArea;

    // Be careful with raising this number high as it can cause very slow frame rates.
    [Range(0, 4)]
    public int repeatAmount = 2;
    
    private void Start() {

        // Grab reference's to some components.
        Vector3 hubWorldPosition = hubWorldMainArea.transform.position;
        Quaternion hubWorldRotation = hubWorldMainArea.transform.rotation;
        float hubBoxAreaSize = hubBoxArea.GetComponent<BoxCollider>().size.x;
        float difference = hubBoxAreaSize;

        for (int i = 0; i < repeatAmount; i++) {

            // ----------------------------------------------------------------------------------------------------------------------------------------------------------
            // X Axis.
            
            float xDif = hubBoxAreaSize;
            
            Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x + difference, hubWorldPosition.y, hubWorldPosition.z), hubWorldRotation);
            Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x - difference, hubWorldPosition.y, hubWorldPosition.z), hubWorldRotation);

            for (int x = 0; x < repeatAmount; x++) {
                
                Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x - difference, hubWorldPosition.y - xDif, hubWorldPosition.z), hubWorldRotation);
                Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x + difference, hubWorldPosition.y - xDif, hubWorldPosition.z), hubWorldRotation);
                Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x + difference, hubWorldPosition.y + xDif, hubWorldPosition.z), hubWorldRotation);

                xDif += hubBoxAreaSize;
            }
            

            // ----------------------------------------------------------------------------------------------------------------------------------------------------------
            // Y Axis.
            
            float yDif = hubBoxAreaSize;
            
            Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x, hubWorldPosition.y + difference, hubWorldPosition.z), hubWorldRotation);
            Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x, hubWorldPosition.y - difference, hubWorldPosition.z), hubWorldRotation);

            for (int y = 0; y < repeatAmount; y++) {
                
                Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x - yDif, hubWorldPosition.y - difference, hubWorldPosition.z), hubWorldRotation);
                Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x - yDif, hubWorldPosition.y + difference, hubWorldPosition.z), hubWorldRotation);

                yDif += hubBoxAreaSize;
            }
            
            // ----------------------------------------------------------------------------------------------------------------------------------------------------------
            // Z Axis.
            
           float zDif = hubBoxAreaSize;
            
            Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x, hubWorldPosition.y, hubWorldPosition.z + difference), hubWorldRotation);
            Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x, hubWorldPosition.y, hubWorldPosition.z - difference), hubWorldRotation);

            for (int z = 0; z < repeatAmount; z++) {

                Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x + zDif, hubWorldPosition.y, hubWorldPosition.z + difference), hubWorldRotation);
                Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x - zDif, hubWorldPosition.y, hubWorldPosition.z + difference), hubWorldRotation);
                Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x + zDif, hubWorldPosition.y, hubWorldPosition.z - difference), hubWorldRotation);
                Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x - zDif, hubWorldPosition.y, hubWorldPosition.z - difference), hubWorldRotation);
                
                Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x, hubWorldPosition.y + zDif, hubWorldPosition.z + difference), hubWorldRotation);
                Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x, hubWorldPosition.y - zDif, hubWorldPosition.z + difference), hubWorldRotation);
                Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x, hubWorldPosition.y + zDif, hubWorldPosition.z - difference), hubWorldRotation);
                Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x, hubWorldPosition.y - zDif, hubWorldPosition.z - difference), hubWorldRotation);

                float innerZDif = hubBoxAreaSize;
                
                for (int innerZ = 0; innerZ < repeatAmount; innerZ++) {
                    
                    Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x + innerZDif, hubWorldPosition.y + zDif, hubWorldPosition.z + difference), hubWorldRotation);
                    Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x - innerZDif, hubWorldPosition.y + zDif, hubWorldPosition.z + difference), hubWorldRotation);
                    Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x + innerZDif, hubWorldPosition.y - zDif, hubWorldPosition.z + difference), hubWorldRotation);
                    Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x - innerZDif, hubWorldPosition.y - zDif, hubWorldPosition.z + difference), hubWorldRotation);
                    
                    Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x + innerZDif, hubWorldPosition.y + zDif, hubWorldPosition.z - difference), hubWorldRotation);
                    Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x - innerZDif, hubWorldPosition.y + zDif, hubWorldPosition.z - difference), hubWorldRotation);
                    Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x + innerZDif, hubWorldPosition.y - zDif, hubWorldPosition.z - difference), hubWorldRotation);
                    Instantiate(hubWorldMainArea, new Vector3(hubWorldPosition.x - innerZDif, hubWorldPosition.y - zDif, hubWorldPosition.z - difference), hubWorldRotation);
                    

                    innerZDif += hubBoxAreaSize;
                }
                
                zDif += hubBoxAreaSize;
            }

            difference += hubBoxAreaSize;
        }
    }
}
