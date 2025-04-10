using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    [Header("References")]
    public GhostSpawner ghostSpawner;
    public OpenElevator openElevatorLeft;
    public OpenElevator openElevatorRight;
    public Transform area;
    public Transform shrinkPoint;

    [Header("Score Settings")]
    public int addscore = 1;

    [Header("Ghost Purge Settings")]
    public float ghostPurgeSpeed = 2f;
    public float ghostShrinkDuration = 1f;
    public LayerMask elevatorLayer;

    private bool isPurging = false;
    private List<GameObject> activeGhosts = new List<GameObject>();
    private List<GameObject> currentlyPurging = new List<GameObject>();

    public static GhostManager instance;

    [Header("Ghost Dialog System")]
    public GameObject[] dialogueObjects; // 场景中摆好的台词对象，默认 inactive


    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (ghostSpawner == null || openElevatorLeft == null || openElevatorRight == null || shrinkPoint == null) return;

        bool eitherDoorOpen = openElevatorLeft.IsAtOpenPosition() || openElevatorRight.IsAtOpenPosition();
        bool bothDoorsClosed = !openElevatorLeft.IsAtOpenPosition() && !openElevatorRight.IsAtOpenPosition();

        if (eitherDoorOpen)
        {
            ghostSpawner.enabled = false;

            if (!isPurging)
            {
                isPurging = true;
                PurgeAllGhosts();
            }
        }
        else if (bothDoorsClosed)
        {
            ghostSpawner.enabled = true;

            if (isPurging)
            {
                isPurging = false;

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

    void PurgeAllGhosts()
    {
        UpdateActiveGhostsList();

        foreach (GameObject ghost in activeGhosts)
        {
            if (ghost != null && !currentlyPurging.Contains(ghost))
            {
                Ghost ghostScript = ghost.GetComponent<Ghost>();
                if (ghostScript != null)
                {
                    ghostScript.enabled = false;
                }

                currentlyPurging.Add(ghost);
                StartCoroutine(MoveAndShrinkGhost(ghost));
            }
        }

        activeGhosts.Clear();
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
        if (ghost == null) yield break;

        Vector3 startScale = ghost.transform.localScale;
        float t = 0f;
        Collider2D ghostCollider = ghost.GetComponent<Collider2D>();

        while (ghost != null && Vector3.Distance(ghost.transform.position, shrinkPoint.position) > 0.1f)
        {
            if (!isPurging)
            {
                if (ghost != null) ghost.GetComponent<Ghost>().enabled = true;
                yield break;
            }

            // Check collision with elevator
            if (ghostCollider != null && Physics2D.OverlapCircle(ghost.transform.position, 0.2f, elevatorLayer))
            {
                if (ghost != null) ghost.GetComponent<Ghost>().enabled = true;
                yield break;
            }

            ghost.transform.position = Vector3.MoveTowards(
                ghost.transform.position,
                shrinkPoint.position,
                ghostPurgeSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Shrink
        t = 0f;
        while (ghost != null && t < ghostShrinkDuration)
        {
            if (!isPurging)
            {
                SpawnRandomDialogue();
                Destroy(ghost);
                GameManager.instance.AddScore(addscore);
                GameManager.instance.playGhostOutSound();
                yield break;
            }

            t += Time.deltaTime;
            float scale = Mathf.Lerp(1f, 0f, t / ghostShrinkDuration);
            ghost.transform.localScale = startScale * scale;
            yield return null;
        }

        if (ghost != null && isPurging)
        {
            SpawnRandomDialogue();
            Destroy(ghost);
            GameManager.instance.AddScore(addscore);
            GameManager.instance.playGhostOutSound();
        }
    }

    public void SetElevatorDoorsMovable(bool canMove)
    {
        if (openElevatorLeft != null) openElevatorLeft.canMove = canMove;
        if (openElevatorRight != null) openElevatorRight.canMove = canMove;
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

    void SpawnRandomDialogue()
    {
        if (dialogueObjects.Length == 0) return;

        List<GameObject> inactiveList = new List<GameObject>();
        foreach (var obj in dialogueObjects)
        {
            if (obj != null && !obj.activeSelf)
            {
                inactiveList.Add(obj);
            }
        }

        if (inactiveList.Count == 0) return;

        GameObject selected = inactiveList[Random.Range(0, inactiveList.Count)];
        selected.SetActive(true);
    }

    public void HideAllDialogues()
    {
        foreach (var obj in dialogueObjects)
        {
            if (obj != null && obj.activeSelf)
            {
                obj.SetActive(false);
            }
        }
    }


}
