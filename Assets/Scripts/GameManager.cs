using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Manager")]
    public GameObject player;
    public int currentFloor;

    [Header("Audio Clips")]
    public AudioClip elevatorStopClip;
    public AudioClip elevatorAlarmClip;
    public AudioClip elevatorStartClip;
    public AudioClip elevatorMovingClip;

    [Header("Particles")]
    public ParticleSystem speedLinesParticleLeft;
    public ParticleSystem speedLinesParticleRight;

    [Header("Score")]
    public int score;
    public enum CurrentState
    {
        Stopped,
        Moving,
        Warning
    }
    private CurrentState _currentState;
    
    // when state changes, call OnStateChanged 
    public CurrentState currentState
    {
        get => _currentState;
        set
        {
            if (_currentState != value)
            {
                _currentState = value;
                OnStateChanged();
            }
        }
    }

    private AudioSource longAudioSource;
    private AudioSource shortAudioSource;

    [Header("Floor Display")]
    public TextMeshProUGUI floorDisplayText;
    public float floorChangeInterval = 20f; // 每20秒改变一次楼层
    public int floorsPerChange = 3; // 每次改变3层
    private float floorChangeTimer = 0f;
    private int targetFloor = 0;

    // initialize the game manager
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
        longAudioSource = gameObject.AddComponent<AudioSource>();
        shortAudioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        score = 0;
        currentState = CurrentState.Stopped;
        StartCoroutine(StartWarning());
    }

    void Update()
    {
        switch (currentState)
        {
            case CurrentState.Moving:
                MoveElevator();
                UpdateFloorDisplay();
                break;
            case CurrentState.Stopped:
                break;
            case CurrentState.Warning:
                break;
        }
    }

    // called once when the state changes
    private void OnStateChanged()
    {
        switch (currentState)
        {
            case CurrentState.Stopped:
                StartCoroutine(StartStopping());
                break;
            case CurrentState.Moving:
                StartCoroutine(StartMoving());
                break;
            case CurrentState.Warning:
                StartCoroutine(StartWarning());
                break;
        }
    }

    #region state functions

    void MoveElevator()
    {
        //TODO: play elevator moving sound
    }

    IEnumerator StartStopping()
    {
        shortAudioSource.PlayOneShot(elevatorStopClip);
        GhostManager.instance.ForceMoveElevators(true);
        yield return new WaitForSeconds(15f);
        currentState = CurrentState.Warning;
    }
    IEnumerator StartWarning()
    {   
        shortAudioSource.PlayOneShot(elevatorAlarmClip, 0.15f);
        yield return new WaitForSeconds(1f);
        GhostManager.instance.ForceMoveElevators(false);
        yield return new WaitForSeconds(2f);
        shortAudioSource.Stop();
        yield return new WaitForSeconds(2f);
        currentState = CurrentState.Moving;
    }
    IEnumerator StartMoving()
    {
        // sound
        shortAudioSource.PlayOneShot(elevatorStartClip);
        longAudioSource.clip = elevatorMovingClip;
        longAudioSource.loop = true;
        longAudioSource.Play();
        StartCoroutine(FadeInAudio(longAudioSource, 2f, 0f, 0.2f));
        // particles
        speedLinesParticleLeft.Play();
        speedLinesParticleRight.Play();
        yield return new WaitForSeconds(60f);
        longAudioSource.Stop();
        speedLinesParticleLeft.Stop();
        speedLinesParticleRight.Stop();
        currentState = CurrentState.Stopped;
    }
    #endregion

    #region helper functions
    IEnumerator FadeInAudio(AudioSource audioSource, float duration, float startVolume = 0f, float targetVolume = 1f)
    {
        float currentTime = 0;
        while (currentTime < duration)  
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            yield return null;
        }
        audioSource.volume = targetVolume;
    }
    
                                                                                                                                                            
    #endregion
    #region score functions
    public void AddScore(int amount)
    {
        score += amount;
    }
    public void SubtractScore(int amount)
    {
        score -= amount;
    }
    #endregion


    /// <summary>
    /// should be implemented in the elevator script
    /// </summary>
    void DoorOpen()
    {

    }

    void DoorClose()
    {

    }

    private void UpdateFloorDisplay()
    {
        if (currentState == CurrentState.Moving)
        {
            floorChangeTimer += Time.deltaTime;
            if (floorChangeTimer >= floorChangeInterval)
            {
                floorChangeTimer = 0f;
                currentFloor -= floorsPerChange;
                targetFloor = currentFloor;
                StartCoroutine(AnimateFloorChange());
            }
        }
    }

    private IEnumerator AnimateFloorChange()
    {
        // 创建新的数字对象
        GameObject newNumber = new GameObject("FloorNumber");
        newNumber.transform.SetParent(floorDisplayText.transform.parent);
        TextMeshProUGUI newText = newNumber.AddComponent<TextMeshProUGUI>();
        newText.text = targetFloor.ToString();
        newText.font = floorDisplayText.font;
        newText.fontSize = floorDisplayText.fontSize;
        newText.color = floorDisplayText.color;
        newText.alignment = floorDisplayText.alignment;
        
        // 设置初始位置（在下方）
        RectTransform rectTransform = newText.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, -50);
        
        // 动画效果
        float duration = 1f;
        float elapsed = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 endPos = Vector2.zero;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }
        
        // 更新显示文本并销毁动画对象
        floorDisplayText.text = targetFloor.ToString();
        Destroy(newNumber);
    }
}
