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
        if (Input.GetMouseButtonDown(0))
        {
            if (interactionZone != null && player != null)
            {
                Collider2D zoneCollider = interactionZone.GetComponent<Collider2D>();
                if (zoneCollider != null && zoneCollider.OverlapPoint(player.transform.position))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                    if (hit.collider != null && hit.collider.gameObject == button)
                    {
                        if (!isMovingOut && !hasReachedTarget)
                        {
                            isMovingOut = true;
                            isReturning = false;
                            waitTimer = 0f;
                        }
                        else if (hasReachedTarget)
                        {
                            hasReachedTarget = false;
                            waitTimer = 0f;
                        }
                    }
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
            }
        }
    }
}

