using UnityEngine;

public class Ghost : MonoBehaviour
{
    [HideInInspector] public GameObject player;
    [HideInInspector] public Transform area;
    public float speed = 1f;
    public float chaseRange = 0.5f;
    public float moveRange = 0.5f;
    public int reducescore = 1;
    public float pauseDuration = 1f;

    private Animator animator;
    private Vector3 originalScale;
    private Vector2 targetPoint;
    private bool isChasing = false;
    private float pauseTimer = 0f;

    private void Start()
    {
        if (area != null) SetRandomTargetPoint();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (player == null || area == null) return;

        if (pauseTimer > 0f)
        {
            animator.SetBool("isTouching", true);
            pauseTimer -= Time.deltaTime;
            return;
        }
        else
        {
            animator.SetBool("isTouching", false);
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        isChasing = distanceToPlayer <= chaseRange;

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            MoveRandomly();
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
        
        //if moving to the right, flip the sprite
        if (transform.position.x < targetPoint.x)
        {
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
        }
        else
        {
            transform.localScale = originalScale;
        }

        if (Vector2.Distance(transform.position, targetPoint) <= 0.1f)
        {
            SetRandomTargetPoint();
        }
    }

    private void ChasePlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        
        //if player on the right side, flip the sprite
        if (player.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
        }
        else
        {
            transform.localScale = originalScale;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            GameManager.instance.playTouchedSound();
            GameManager.instance.SubtractScore(reducescore);
            pauseTimer = pauseDuration;
        }
    }

}


