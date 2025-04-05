using UnityEngine;

public class OpenElevator : MonoBehaviour
{
    public GameObject button;
    public GameObject door;
    public GameObject player;
    public GameObject interactionZone;
    public float moveDistance = 1f;
    public float moveSpeed = 1f;

    private Vector3 originalPos;
    private Vector3 targetPos;
    private bool isMovingOut = false;
    private bool isReturning = false;
    private bool hasReachedTarget = false;
    private float waitTimer = 0f;
    private float waitDuration = 1f;
    private bool hasActivated = false;

    void Start()
    {
        if (door != null)
        {
            originalPos = door.transform.position;
            targetPos = originalPos + Vector3.right * moveDistance;
        }
    }

    void Update()
    {
        if (interactionZone != null && player != null)
        {
            Collider2D zoneCollider = interactionZone.GetComponent<Collider2D>();
            if (zoneCollider != null && zoneCollider.OverlapPoint(player.transform.position))
            {
                if (!hasActivated && !isMovingOut && !hasReachedTarget)
                {
                    isMovingOut = true;
                    isReturning = false;
                    hasActivated = true;
                    waitTimer = 0f;
                }
            }
        }

        if (isMovingOut)
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, targetPos, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(door.transform.position, targetPos) < 0.01f)
            {
                door.transform.position = targetPos;
                isMovingOut = false;
                hasReachedTarget = true;
                waitTimer = 0f;
            }
        }

        if (hasReachedTarget)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitDuration)
            {
                isReturning = true;
                hasReachedTarget = false;
            }
        }

        if (isReturning)
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, originalPos, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(door.transform.position, originalPos) < 0.01f)
            {
                door.transform.position = originalPos;
                isReturning = false;
                hasActivated = false;
            }
        }
    }
}
