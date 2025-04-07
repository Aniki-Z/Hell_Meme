using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public static TextManager Instance;

    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _dialogueText;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        _scoreText.text = GameManager.instance.score.ToString();
    }
}
