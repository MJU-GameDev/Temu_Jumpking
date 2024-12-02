
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Vector2 minBounds;
    public Vector2 maxBounds;
    public float smoothTime = 0.2f;
    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPosition = new Vector3(player.position.x, player.position.y, transform.position.z);

        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
