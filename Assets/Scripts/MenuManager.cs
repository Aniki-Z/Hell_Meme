using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    public GameObject button;
    public Sprite buttonSprite;
    public Sprite buttonPressedSprite;
    public Image fadeImage;
    public float fadeDuration = 3f;

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
    void Start()
    {
        button.GetComponent<SpriteRenderer>().sprite = buttonSprite;
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }

    public void ButtonPressed(GameObject button)
    {
        button.GetComponent<SpriteRenderer>().sprite = buttonPressedSprite;
        StartCoroutine(FadeToBlackAndLoadScene());
    }

    public void ButtonReleased(GameObject button)
    {
        button.GetComponent<SpriteRenderer>().sprite = buttonSprite;
    }

    private IEnumerator FadeToBlackAndLoadScene()
    {
        if (fadeImage == null)
        {
            yield break;
        }

        float elapsedTime = 0f;
        Color startColor = new Color(0, 0, 0, 0);
        Color endColor = new Color(0, 0, 0, 1);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        fadeImage.color = endColor;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
