using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControllerManager : MonoBehaviour
{
    public static MouseControllerManager Instance; 
    [SerializeField] private GameObject _colliderPrefab;
    [SerializeField] private GameObject _effectPrefab;
    [SerializeField] private List<Collider2D> _colliders;
    private List<GameObject> currColliders = new List<GameObject>();
    private List<GameObject> currEffects = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameManager.instance.playSlapSound();
            Vector3 mousePositionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePositionInWorld, Vector2.zero);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                StartCoroutine(SpawnWithDelay(mousePositionInWorld, hit.collider));
            }
            else
            {
                currColliders.Add(GameObject.Instantiate(_colliderPrefab, mousePositionInWorld, Quaternion.identity));
                currEffects.Add(GameObject.Instantiate(_effectPrefab, mousePositionInWorld, Quaternion.identity));
                Invoke("destroyPrefab", 0.3f);
            }


        }
    }

    private void destroyPrefab()
    {
        for (int i = 0; i < currColliders.Count; i++)
        {
            Destroy(currColliders[i]);
            Destroy(currEffects[i]);
        }
        currColliders = new List<GameObject>();
        currEffects = new List<GameObject>();
    }

    private IEnumerator SpawnWithDelay(Vector3 mouseWorldPos, Collider2D hitCollider)
    {
        double minDist = double.MaxValue;
        Collider2D closestCollider = hitCollider;

        foreach (Collider2D collider in _colliders)
        {
            double dist = Vector2.Distance(mouseWorldPos, collider.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestCollider = collider;
            }
        }

        // Spawn at closest bone first
        //currColliders.Add(Instantiate(_colliderPrefab, mouseWorldPos, Quaternion.identity));

        currColliders.Add(GameObject.Instantiate(_colliderPrefab, closestCollider.transform.position, Quaternion.identity));
        currEffects.Add(GameObject.Instantiate(_effectPrefab, closestCollider.transform.position, Quaternion.identity));
        Invoke("destroyPrefab", 0.3f);
        //  wait 0.2 seconds before the second one
        yield return new WaitForSeconds(1f);

    }

}