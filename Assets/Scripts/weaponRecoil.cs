using UnityEngine;

public class weaponRecoil : MonoBehaviour
{
    [Header("---- Recoil Settings ----")]
    [SerializeField] float recoilAmount = 5f;
    [SerializeField] float recoilReturnSpeed = 8f;
    [SerializeField] float recoilSnappiness = 10f;

    Vector3 currentRotation;
    Vector3 targetRotation;
    Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, Time.deltaTime * recoilReturnSpeed);
        currentRotation = Vector3.Lerp(currentRotation, targetRotation, Time.deltaTime * recoilSnappiness);

        transform.localRotation = startRotation * Quaternion.Euler(currentRotation);
    }

    public void Fire()
    {
        targetRotation += new Vector3(recoilAmount, Random.Range(-1f, 1f), 0f);
    }
}