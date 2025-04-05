using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    public GameObject ghostPrefab;
    public Transform player;
    public Transform area;

    public float minSpawnInterval = 3f;
    public float maxSpawnInterval = 5f;

    private float timer;

    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnGhost();
            ResetTimer();
        }
    }

    void ResetTimer()
    {
        timer = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    void SpawnGhost()
    {
        Vector3 spawnPosition = GetSpawnPositionNearCamera();
        GameObject newGhost = Instantiate(ghostPrefab, spawnPosition, Quaternion.identity);

        Ghost ghostScript = newGhost.GetComponent<Ghost>();
        ghostScript.player = player.gameObject;
        ghostScript.area = area;
    }

    Vector3 GetSpawnPositionNearCamera()
    {
        Camera cam = Camera.main;
        Vector2 screenEdgeOffset = Random.insideUnitCircle.normalized;

        Vector3 camCenter = cam.transform.position;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float x = camCenter.x + screenEdgeOffset.x * (camWidth / 2f + 1f);
        float y = camCenter.y + screenEdgeOffset.y * (camHeight / 2f + 1f);

        return new Vector3(x, y, 0f);
    }
}
