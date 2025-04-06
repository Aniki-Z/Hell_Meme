using UnityEngine;

public class OpenElevator : MonoBehaviour
{
    public GameObject button;
    public GameObject door;
    public GameObject interactionZone;
    public float moveDistance = 1f;
    public float moveSpeed = 1f;
    public bool canMove = true;

    private Vector3 originalPos;
    private Vector3 targetPos;
    private bool isMovingOut = false;
    private bool isReturning = false;
    private bool hasReachedTarget = false;
    private float waitTimer = 0f;
    private float waitDuration = 1f;
    private bool hasActivated = false;

    private bool playerInside = false;
    private float reopenDelayTimer = 0f;
    private bool shouldReopen = false;
    private float reopenDelay = 0.2f;

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
        if (!canMove) return;

        playerInside = false;
        if (interactionZone != null)
        {
            Collider2D zoneCollider = interactionZone.GetComponent<Collider2D>();
            if (zoneCollider != null)
            {
                Collider2D[] colliders = Physics2D.OverlapBoxAll(zoneCollider.bounds.center, zoneCollider.bounds.size, 0f);
                foreach (var col in colliders)
                {
                    if (col.CompareTag("Player"))
                    {
                        playerInside = true;

                        if (isReturning)
                        {
                            isReturning = false;
                            shouldReopen = true;
                            reopenDelayTimer = 0f;
                        }

                        if (!isMovingOut && !hasReachedTarget && !shouldReopen)
                        {
                            isMovingOut = true;
                            isReturning = false;
                            hasActivated = true;
                            waitTimer = 0f;
                        }

                        break;
                    }
                }
            }
        }

        if (shouldReopen)
        {
            reopenDelayTimer += Time.deltaTime;
            if (reopenDelayTimer >= reopenDelay)
            {
                shouldReopen = false;
                isMovingOut = true;
                isReturning = false;
                hasActivated = true;
                waitTimer = 0f;
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
            if (!playerInside)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer >= waitDuration)
                {
                    isReturning = true;
                    hasReachedTarget = false;
                }
            }
            else
            {
                waitTimer = 0f;
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

    public bool IsDoorMoving()
    {
        return isMovingOut || isReturning;
    }
}
