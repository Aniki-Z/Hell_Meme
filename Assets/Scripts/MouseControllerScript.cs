using System.Collections.Generic;
using UnityEngine;

public class MouseControllerScript : MonoBehaviour
{
    [SerializeField] private GameObject _colliderPrefab;
    [SerializeField] private GameObject _effectPrefab;
    private List<GameObject> currColliders = new List<GameObject>();
    private List<GameObject> currEffects = new List<GameObject>();




    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePositionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currColliders.Add(GameObject.Instantiate(_colliderPrefab, mousePositionInWorld, Quaternion.identity));
            currEffects.Add(GameObject.Instantiate(_effectPrefab, mousePositionInWorld, Quaternion.identity));

            Invoke("destroyPrefab", 0.3f);
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
}
