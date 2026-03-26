using UnityEngine;

public class cameraController : MonoBehaviour
{
    [Header("---- Camera Look ----")]
    [SerializeField] float sens = 120f;
    [SerializeField] int lockVertMin = -70;
    [SerializeField] int lockVertMax = 70;
    [SerializeField] bool invertY = false;
    [SerializeField] Transform player;

    [Header("---- Camera Bob ----")]
    [SerializeField] CharacterController controller;
    [SerializeField] float bobSpeed = 6f;
    [SerializeField] float bobAmount = 0.05f;
    [SerializeField] float bobReturnSpeed = 5f;

    float camRotX;
    float defaultY;
    float defaultX;
    float bobTimer;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        defaultY = transform.localPosition.y;
        defaultX = transform.localPosition.x;
    }

    void Update()
    {
        if (gamemanager.instance != null && gamemanager.instance.isPaused)
        {
            return;
        }

        HandleLook();
        HandleCameraBob();
    }

    void HandleLook()
    {
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

    void HandleCameraBob()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        bool isMoving = Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;

        if (controller != null && controller.isGrounded && isMoving)
        {
            bobTimer += Time.deltaTime * bobSpeed;

            float currentBobAmount = bobAmount;

            // Sprint = stronger bob
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentBobAmount *= 1.5f;
            }

            float newY = defaultY + Mathf.Sin(bobTimer) * currentBobAmount;
            float newX = defaultX + Mathf.Cos(bobTimer * 0.5f) * (currentBobAmount * 0.5f);

            transform.localPosition = new Vector3(newX, newY, transform.localPosition.z);
        }
        else
        {
            bobTimer = 0f;

            float smoothX = Mathf.Lerp(transform.localPosition.x, defaultX, Time.deltaTime * bobReturnSpeed);
            float smoothY = Mathf.Lerp(transform.localPosition.y, defaultY, Time.deltaTime * bobReturnSpeed);

            transform.localPosition = new Vector3(smoothX, smoothY, transform.localPosition.z);
        }
    }
}