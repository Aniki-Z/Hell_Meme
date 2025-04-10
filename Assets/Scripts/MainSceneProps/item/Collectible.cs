using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int scoreValue = 10;
    private bool collected = false; // 防止重复触发

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        collected = true;

        // 禁用 Collider，防止后续触发
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 加分并生成下一个
        GameManager.instance.AddScore(scoreValue);
        GameManager.instance.SpawnNewCollectible();

        // 延迟销毁（可以看特效，不想延迟就直接 Destroy）
        Destroy(gameObject);
    }
}
