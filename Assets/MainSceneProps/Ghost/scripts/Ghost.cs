using UnityEngine;

public class Ghost : MonoBehaviour
{
    public GameObject player;
    public Transform area;
    public float speed = 1f;
    public float chaseRange = 0.5f;
    public float moveRange = 0.5f;
    private Vector2 targetPoint;
    private bool isChasing = false;

    private void Start()
    {
        SetRandomTargetPoint();
    }

    private void Update()
    {
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            MoveRandomly();
        }

        if (Vector2.Distance(transform.position, player.transform.position) <= chaseRange)
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }
    }

    private void SetRandomTargetPoint()
    {
        float x = Random.Range(area.position.x - area.localScale.x / 2, area.position.x + area.localScale.x / 2);
        float y = Random.Range(area.position.y - area.localScale.y / 2, area.position.y + area.localScale.y / 2);
        targetPoint = new Vector2(x, y);
    }

    private void MoveRandomly()
    {
        Vector2 randomPosition = targetPoint + new Vector2(Random.Range(-moveRange, moveRange), Random.Range(-moveRange, moveRange));
        transform.position = Vector2.MoveTowards(transform.position, randomPosition, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint) <= 0.1f)
        {
            SetRandomTargetPoint();
        }
    }

    private void ChasePlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, player.transform.position) > chaseRange)
        {
            isChasing = false;
            SetRandomTargetPoint();
        }
    }
}

