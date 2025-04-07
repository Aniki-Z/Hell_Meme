using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
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
    public bool isNormalFloor => currentFloor > -17;
    
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
    public TextMeshPro floorDisplayText;
    public float floorChangeInterval = 20f;
    public int floorsPerChange = 1;
    public Vector2 newNumberStartPos = new Vector2(0, -50);
    public float animationDuration = 3f;
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
        //play stop sound
        shortAudioSource.PlayOneShot(elevatorStopClip);

        //open the door
        GhostManager.instance.ForceMoveElevators(true);

        //set the doors movable
        GhostManager.instance.SetElevatorDoorsMovable(true);

        if (isNormalFloor)
        {
            yield return new WaitForSeconds(15f);

            //set the doors unmovable
            GhostManager.instance.SetElevatorDoorsMovable(false);

            currentState = CurrentState.Warning;
        }
        else
        {   
            // keep the fxxking door open
            int i = 0;
            while(i<5)
            {
                yield return new WaitForSeconds(1f);
                GhostManager.instance.ForceMoveElevators(true);
                i++;
            }

            // end the game
            if (score > 10)
            {
                SceneManager.LoadScene("SuccessEnd");
            }
            else
            {
                SceneManager.LoadScene("FailedEnd");
            }
        }
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

        if (isNormalFloor)
        {
            yield return new WaitForSeconds(60f);
        }
        else
        {
            yield return new WaitForSeconds(10f);
        }
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

    #region floor display
    private void UpdateFloorDisplay()
    {
        floorChangeTimer += Time.deltaTime;
        if (floorChangeTimer >= (isNormalFloor ? floorChangeInterval : 10f))
        {
            floorChangeTimer = 0f;
            currentFloor -= floorsPerChange;
            targetFloor = currentFloor;
            StartCoroutine(AnimateFloorChange());
        }
    }

    private IEnumerator AnimateFloorChange()
    {
        // create a new floor number
        GameObject newNumber = new GameObject("FloorNumber");
        newNumber.transform.SetParent(floorDisplayText.transform.parent);
        TextMeshPro newText = newNumber.AddComponent<TextMeshPro>();
        newText.text = targetFloor.ToString();
        newText.font = floorDisplayText.font;
        newText.fontSize = floorDisplayText.fontSize;
        newText.color = floorDisplayText.color;
        newText.alignment = floorDisplayText.alignment;
        
        // get the old floor number's RectTransform
        RectTransform oldRectTransform = floorDisplayText.GetComponent<RectTransform>();
        RectTransform newRectTransform = newText.GetComponent<RectTransform>();
        
        // set the initial position of the new floor number
        newRectTransform.anchoredPosition = newNumberStartPos;
        
        // animation
        float elapsed = 0f;
        Vector2 oldStartPos = oldRectTransform.anchoredPosition;
        float moveDistance = oldStartPos.y - newNumberStartPos.y;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            
            // floor number moves up
            oldRectTransform.anchoredPosition = Vector2.Lerp(oldStartPos, 
            new Vector2(oldStartPos.x, oldStartPos.y + moveDistance), t);
            newRectTransform.anchoredPosition = Vector2.Lerp(newNumberStartPos, oldStartPos, t);
            
            yield return null;
        }
        
        // update the display text and destroy the animation object
        floorDisplayText.text = targetFloor.ToString(); 
        floorDisplayText.GetComponent<RectTransform>().anchoredPosition = oldStartPos;
        Destroy(newNumber);
    }
    #endregion
}
