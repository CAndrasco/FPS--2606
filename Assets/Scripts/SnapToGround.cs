using UnityEngine;

public class SnapToGround : MonoBehaviour
{
    void Start()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position + Vector3.up * 50f, Vector3.down, 200f);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Floor"))
            {
                float height = GetObjectHeight();

                Vector3 pos = transform.position;
                pos.y = hit.point.y + (height / 2f);

                transform.position = pos;
                break;
            }
        }
    }

    float GetObjectHeight()
    {
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
            return rend.bounds.size.y;

        return 1f;
    }
}