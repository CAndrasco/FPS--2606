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
