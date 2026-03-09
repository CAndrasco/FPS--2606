using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sens = 120;
    [SerializeField] int lockVertMin = -70;
    [SerializeField] int lockVertMax = 70;
    [SerializeField] bool invertY = false;
    [SerializeField] Transform player;

    float camRotX;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (gamemanager.instance != null && gamemanager.instance.isPaused)
        {
            return;
        }

        float mouseX = Input.GetAxisRaw("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sens * Time.deltaTime;

        if (invertY)
        {
            camRotX += mouseY;
        }
        else
        {
            camRotX -= mouseY;
        }

        camRotX = Mathf.Clamp(camRotX, lockVertMin, lockVertMax);

        transform.localRotation = Quaternion.Euler(camRotX, 0, 0);

        player.Rotate(Vector3.up * mouseX);
    }
}