using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Manager")]
    public GameObject player;
    public GameObject leftDoor;
    public GameObject rightDoor;

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
        DoorOpen();
        yield return new WaitForSeconds(15f);
        currentState = CurrentState.Warning;
    }
    IEnumerator StartWarning()
    {   
        shortAudioSource.PlayOneShot(elevatorAlarmClip, 0.15f);
        yield return new WaitForSeconds(1f);
        DoorClose();
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
    void AddScore(int amount)
    {
        score += amount;
    }
    void SubtractScore(int amount)
    {
        score -= amount;
    }
    #endregion

    #region elevator functions

    /// <summary>
    /// should be implemented in the elevator script
    /// </summary>
    void DoorOpen()
    {

    }

    void DoorClose()
    {

    }

    #endregion

}
