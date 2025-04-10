using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int scoreValue = 10;
    private bool collected = false; // ��ֹ�ظ�����

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        collected = true;

        // ���� Collider����ֹ��������
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // �ӷֲ�������һ��
        GameManager.instance.AddScore(scoreValue);
        GameManager.instance.SpawnNewCollectible();

        // �ӳ����٣����Կ���Ч�������ӳپ�ֱ�� Destroy��
        Destroy(gameObject);
    }
}
