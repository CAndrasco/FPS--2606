using UnityEngine;

public class miniMapFollow : MonoBehaviour
{
    public string floorTag = "Floor";
    [SerializeField] Transform player;
    private Camera mapCam;
    private float minX, maxX, minZ, maxZ;

    void Start()
    {
        mapCam = GetComponent<Camera>();
        
        //Grab player from gamemanager
        if(gamemanager.instance != null && gamemanager.instance.player != null)
        {
            player = gamemanager.instance.player.transform;
        }

        GameObject floor = GameObject.FindWithTag(floorTag);
        Bounds b = floor.GetComponent<Renderer>().bounds;

        float halfHeight = mapCam.orthographicSize;
        float halfWidth = halfHeight;

        minX = b.min.x + halfWidth;
        maxX = b.max.x - halfWidth;
        minZ = b.min.z + halfHeight;
        maxZ = b.max.z - halfHeight;
    }




    //Logic for minimap to follow player but not rotate
    void LateUpdate()
    {
        
        //calculate position using clamps
        float clampX = Mathf.Clamp(player.position.x, minX, maxX);
        float clampZ = Mathf.Clamp(player.position.z, minZ, maxZ);

        //camera will follow player and stop at the wall. not exceeding playable map
        transform.position = new Vector3(clampX, transform.position.y, clampZ);

        //Force camera not to spin with player
        transform.rotation = Quaternion.Euler(90f,0f,0f);
    }
}
