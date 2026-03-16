using UnityEngine;

public class miniMapFollow : MonoBehaviour
{
    [SerializeField] Transform player;

    void Start()
    {
        //Grab player from gamemanager
        if(gamemanager.instance != null && gamemanager.instance.player != null)
        {
            player = gamemanager.instance.player.transform;
        }

        //create light for map layer
        GameObject lightGameObject = new GameObject("Minimap_Sun");
        Light lightPtr = lightGameObject.AddComponent<Light>();

        lightPtr.type = LightType.Directional;
        lightPtr.intensity = 1.0f;

        //set to only hit map layer
        lightPtr.cullingMask = 1 << LayerMask.NameToLayer("MapLayer");
        
        //have light follow camera
        lightGameObject.transform.parent = transform;
        lightGameObject.transform.localRotation = Quaternion.Euler(90,0,0); 
    }




    //Logic for minimap to follow player but not rotate
    void LateUpdate()
    {
        //Follow players x and z staying at fixed y
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        //Force camera not to spin with player
        transform.rotation = Quaternion.Euler(90f,0f,0f);
    }
}
