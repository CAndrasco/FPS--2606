using UnityEngine;

public class weaponSway : MonoBehaviour
{
    [Header("---- Sway Settings ----")]
    [SerializeField] float swayAmount = 2f;
    [SerializeField] float maxSway = 5f;
    [SerializeField] float smoothSpeed = 8f;

    Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float moveX = Mathf.Clamp(-mouseX * swayAmount, -maxSway, maxSway);
        float moveY = Mathf.Clamp(-mouseY * swayAmount, -maxSway, maxSway);

        Vector3 targetPosition = new Vector3(moveX, moveY, 0);

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            initialPosition + targetPosition,
            Time.deltaTime * smoothSpeed
        );
    }
}