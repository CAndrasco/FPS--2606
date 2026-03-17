using UnityEngine;

public class miniMapFollow : MonoBehaviour
{
    [Header("--Auto-Detection--")]
    [SerializeField] string floorTag = "Floor";

    [SerializeField] Transform player;
    [SerializeField] float minX, minZ, maxX, maxZ;
    [SerializeField] Camera mapCam;

    void Start()
    {
        mapCam = GetComponent<Camera>();

        //Grab player from gamemanager
        if(gamemanager.instance != null && gamemanager.instance.player != null)
        {
            player = gamemanager.instance.player.transform;
        }

        CalculateBoundaries();
    }

    public void CalculateBoundaries()
    {
        //find the floor
        GameObject floor = GameObject.FindWithTag(floorTag);

        if (floor != null)
        {
            Renderer floorRend = floor.GetComponent<Renderer>();
            Bounds b = floorRend.bounds;

            //calculat cam half height/width so camear edge stops at bounds
            float vertExtent = mapCam.orthographicSize;
            float horizExtent = mapCam.orthographicSize;

            minX = b.min.x + horizExtent;
            maxX = b.max.x - horizExtent;
            minZ = b.min.z + vertExtent;
            maxZ = b.max.z - vertExtent;
        }
        else
        {
            Debug.LogError("Minimap Error: No Floor");
        }
    }


    //Logic for minimap to follow player but not rotate
    void LateUpdate()
    {
        //follow player within floor boundaries
        float clampedX = Mathf.Clamp(player.position.x, minX, maxX);
        float clampedZ = Mathf.Clamp(player.position.z, minZ, maxZ);
        transform.position = new Vector3(clampedX, transform.position.y, clampedZ);
        
        //Force camera not to spin with player
        transform.rotation = Quaternion.Euler(90f,0f,0f);
    }
}
