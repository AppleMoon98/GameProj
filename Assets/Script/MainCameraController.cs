using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    Vector2 cameraPosition = new(0, 0);
    Transform playerTransform;
    public Transform backgroundTransform;
    public float yOffset = 2f;

    void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        cameraPosition = playerTransform.position;
        Vector3 velo = Vector3.zero;
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(cameraPosition.x, cameraPosition.y + yOffset, transform.position.z), ref velo, 0.1f);
        backgroundTransform.position = new Vector3(transform.position.x, transform.position.y + yOffset, 1f);
    }
}
