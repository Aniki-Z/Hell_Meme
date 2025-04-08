using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    public GhostSpawner ghostSpawner;
    public OpenElevator openElevatorLeft;
    public OpenElevator openElevatorRight;
    public Transform area;
    public Transform shrinkPoint;
    public int addscore = 1;

    private bool isPurging = false;
    private List<GameObject> activeGhosts = new List<GameObject>();
    private Coroutine purgeCoroutine;
    private List<GameObject> currentlyPurging = new List<GameObject>();

    public static GhostManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (ghostSpawner == null || openElevatorLeft == null || openElevatorRight == null || shrinkPoint == null) return;

        bool eitherDoorMoving = openElevatorLeft.IsDoorMoving() || openElevatorRight.IsDoorMoving();
        bool bothDoorsIdle = !openElevatorLeft.IsDoorMoving() && !openElevatorRight.IsDoorMoving();

        if (eitherDoorMoving)
        {
            ghostSpawner.enabled = false;

            if (!isPurging)
            {
                isPurging = true;
                purgeCoroutine = StartCoroutine(PurgeGhostsOneByOne());
            }
        }
        else if (bothDoorsIdle)
        {
            ghostSpawner.enabled = true;

            if (isPurging)
            {
                isPurging = false;

                if (purgeCoroutine != null)
                {
                    StopCoroutine(purgeCoroutine);
                    purgeCoroutine = null;
                }

                foreach (GameObject ghost in currentlyPurging)
                {
                    if (ghost != null)
                    {
                        Ghost ghostScript = ghost.GetComponent<Ghost>();
                        if (ghostScript != null)
                        {
                            ghostScript.enabled = true;
                        }
                    }
                }

                currentlyPurging.Clear();
            }
        }
    }

    IEnumerator PurgeGhostsOneByOne()
    {
        while (isPurging)
        {
            UpdateActiveGhostsList();

            if (activeGhosts.Count > 0)
            {
                GameObject ghost = activeGhosts[0];
                activeGhosts.RemoveAt(0);

                Ghost ghostScript = ghost.GetComponent<Ghost>();
                if (ghostScript != null)
                {
                    ghostScript.enabled = false;
                }

                currentlyPurging.Add(ghost);

                yield return StartCoroutine(MoveAndShrinkGhost(ghost));

                currentlyPurging.Remove(ghost);

                yield return new WaitForSeconds(0.7f);
            }
            else
            {
                yield return null;
            }
        }
    }

    void UpdateActiveGhostsList()
    {
        activeGhosts.Clear();
        Collider2D[] colliders = Physics2D.OverlapBoxAll(area.position, area.localScale, 0f);
        foreach (var col in colliders)
        {
            if (col.CompareTag("Ghost") && !activeGhosts.Contains(col.gameObject) && !currentlyPurging.Contains(col.gameObject))
            {
                activeGhosts.Add(col.gameObject);
            }
        }
    }

    IEnumerator MoveAndShrinkGhost(GameObject ghost)
    {
        float moveSpeed = 2f;
        float shrinkDuration = 1f;

        if (ghost == null) yield break;

        Vector3 startScale = ghost.transform.localScale;
        float t = 0f;

        while (ghost != null && Vector3.Distance(ghost.transform.position, shrinkPoint.position) > 0.1f)
        {
            if (!isPurging)
            {
                Ghost ghostScript = ghost.GetComponent<Ghost>();
                if (ghostScript != null)
                {
                    ghostScript.enabled = true;
                }
                yield break;
            }

            ghost.transform.position = Vector3.MoveTowards(ghost.transform.position, shrinkPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        if (!isPurging)
        {
            Destroy(ghost);
            GameManager.instance.AddScore(addscore);
            GameManager.instance.playGhostOutSound();
            yield break;
        }

        t = 0f;
        while (ghost != null && t < shrinkDuration)
        {
            if (!isPurging)
            {
                Destroy(ghost);
                GameManager.instance.AddScore(addscore);
                GameManager.instance.playGhostOutSound();
                yield break;
            }

            t += Time.deltaTime;
            float scale = Mathf.Lerp(1f, 0f, t / shrinkDuration);
            ghost.transform.localScale = startScale * scale;
            yield return null;
        }

        if (ghost != null && isPurging)
        {
            Destroy(ghost);
            GameManager.instance.AddScore(addscore);
            GameManager.instance.playGhostOutSound();
        }
    }
    
    //public function below
    public void SetElevatorDoorsMovable(bool canMove)
    {
        if (openElevatorLeft != null)
        {
            openElevatorLeft.canMove = canMove;
        }

        if (openElevatorRight != null)
        {
            openElevatorRight.canMove = canMove;
        }
    }

    public void ForceMoveElevators(bool toTargetPosition)
    {
        if (openElevatorLeft != null)
        {
            if (toTargetPosition) openElevatorLeft.ForceOpen();
            else openElevatorLeft.ForceClose();
        }

        if (openElevatorRight != null)
        {
            if (toTargetPosition) openElevatorRight.ForceOpen();
            else openElevatorRight.ForceClose();
        }
    }
}