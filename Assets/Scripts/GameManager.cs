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
    public AudioClip backgroundMusicClip;
    public AudioClip[] catTouchedClips;
    public AudioClip[] catSlapClip;
    public AudioClip ghostOutClip;
    public AudioClip buttonPressedClip;
    [Header("Particles")]
    public ParticleSystem speedLinesParticleLeft;
    public ParticleSystem speedLinesParticleRight;

    [Header("Buttons")]
    public GameObject leftButton;
    public GameObject rightButton;
    public Sprite buttonSprite;
    public Sprite buttonPressedSprite;

    private IEnumerator glowCoroutineLeft;
    private IEnumerator glowCoroutineRight;
    [Header("Score")]
    public int score;

    [Header("light")]
    public GameObject elevatorStopIndicator;

    [Header("Collectible Settings")]
    public GameObject collectiblePrefab;
    public Vector2 spawnMin = new Vector2(-3.2f, -4f);
    public Vector2 spawnMax = new Vector2(3.2f, -1f);

    public enum CurrentState
    {
        Stopped,
        Moving,
        Warning
    }
    private CurrentState _currentState;
    public bool isNormalFloor => currentFloor >= -17;
    
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

    private AudioSource longElevatorAudio;
    private AudioSource shortElevatorAudio;
    private AudioSource basicAudio;
    private AudioSource backgroundMusic;

    [Header("Floor Display")]
    public TextMeshPro floorDisplayText;
    public float floorChangeInterval = 20f;
    public int floorsPerChange = 1;
    public Vector2 newNumberStartPos = new Vector2(0, -50);
    public float animationDuration = 3f;
    private int targetFloor = 0;

    [Header("Elevator Shake")]
    
    public GameObject elevator;
    public float shakeAmount = 0.1f;
    public float shakeSpeed = 5f;
    private Vector3 originalElevatorPosition;

    [Header("End Screen")]
    public GameObject kojima;
    public Image fadeImage;

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
        longElevatorAudio = gameObject.AddComponent<AudioSource>();
        shortElevatorAudio = gameObject.AddComponent<AudioSource>();
        backgroundMusic = gameObject.AddComponent<AudioSource>();
        basicAudio = gameObject.AddComponent<AudioSource>();
        backgroundMusic.clip = backgroundMusicClip;
        backgroundMusic.volume = 0.1f;
        backgroundMusic.loop = true;
        backgroundMusic.Play();
    }

    void Start()
    {
        SpawnNewCollectible();

        score = 0;
        currentState = CurrentState.Stopped;

        // set the elevator position
        originalElevatorPosition = elevator.transform.position;

        // set the doors unmovable
        GhostManager.instance.SetElevatorDoorsMovable(false);

        StartCoroutine(StartWarning());
    }

    void Update()
    {
        switch (currentState)
        {
            case CurrentState.Moving:
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
        if (elevatorStopIndicator != null)
        {
            if (currentState == CurrentState.Stopped)
            {
                elevatorStopIndicator.SetActive(true);
            }
            else if (currentState == CurrentState.Warning)
            {
                StartCoroutine(BlinkAndTurnOffIndicator());
            }
            else
            {
                elevatorStopIndicator.SetActive(false);
            }
        }

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


    public void SpawnNewCollectible()
    {
        if (collectiblePrefab == null) return;

        Vector2 newPos = new Vector2(
            Random.Range(spawnMin.x, spawnMax.x),
            Random.Range(spawnMin.y, spawnMax.y)
        );

        Instantiate(collectiblePrefab, newPos, Quaternion.identity);
    }

    private IEnumerator BlinkAndTurnOffIndicator(int blinkCount = 3, float blinkInterval = 0.3f)
    {
        for (int i = 0; i < blinkCount; i++)
        {
            elevatorStopIndicator.SetActive(false);
            yield return new WaitForSeconds(blinkInterval);
            elevatorStopIndicator.SetActive(true);
            yield return new WaitForSeconds(blinkInterval);
        }
        elevatorStopIndicator.SetActive(false);
    }

    #region state functions

    // Stop => Warning
    IEnumerator StartStopping()
    {   
        //play stop sound
        shortElevatorAudio.PlayOneShot(elevatorStopClip);

        //open the door
        GhostManager.instance.ForceMoveElevators(true);

        //set the doors movable
        GhostManager.instance.SetElevatorDoorsMovable(true);
        
        // Start button glow effect
        StartButtonGlow();

        if (isNormalFloor)
        {
            yield return new WaitForSeconds(15f);

            //set the doors unmovable
            GhostManager.instance.SetElevatorDoorsMovable(false);
            
            // Stop button glow effect
            StopCoroutine(glowCoroutineLeft);
            StopCoroutine(glowCoroutineRight);
            SpriteRenderer leftGlow = leftButton.transform.GetChild(0).GetComponent<SpriteRenderer>();
            SpriteRenderer rightGlow = rightButton.transform.GetChild(0).GetComponent<SpriteRenderer>();
            if (leftGlow != null) leftGlow.gameObject.SetActive(false);
            if (rightGlow != null) rightGlow.gameObject.SetActive(false);

            currentState = CurrentState.Warning;
        }
        else
        {   
            kojima.SetActive(true);

            // keep the fxxking door open
            int i = 0;
            while(i<5)
            {
                yield return new WaitForSeconds(1f);
                GhostManager.instance.ForceMoveElevators(true);
                i++;
            }

            // end the game
            if (score > 200)
            {
                StartCoroutine(FadeToBlackAndLoadScene("SuccessEnd"));
            }
            else
            {
                StartCoroutine(FadeToBlackAndLoadScene("FailedEnd"));
            }
        }
    }

    // Warning => Moving
    IEnumerator StartWarning()
    {
        GhostManager.instance.HideAllDialogues(); //电梯开始动前，清除台词

        shortElevatorAudio.PlayOneShot(elevatorAlarmClip, 0.15f);
        yield return new WaitForSeconds(1f);
        GhostManager.instance.ForceMoveElevators(false);
        yield return new WaitForSeconds(2f);
        shortElevatorAudio.Stop();
        yield return new WaitForSeconds(1f);
        currentState = CurrentState.Moving;
    }

    // Moving => Stopped
    IEnumerator StartMoving()
    {
        // sound
        shortElevatorAudio.PlayOneShot(elevatorStartClip);
        longElevatorAudio.clip = elevatorMovingClip;
        longElevatorAudio.loop = true;
        longElevatorAudio.Play();
        StartCoroutine(FadeInAudio(longElevatorAudio, 2f, 0f, 0.5f));
        
        // particles
        speedLinesParticleLeft.Play();
        speedLinesParticleRight.Play();

        // Start shaking
        StartCoroutine(ShakeElevator());

        if (isNormalFloor)
        {
            int i = 0;
            while(i< (30 / floorChangeInterval))
            {
                yield return new WaitForSeconds(floorChangeInterval);
                UpdateFloorDisplay();
                i++;
            }
        }
        else
        {
            yield return new WaitForSeconds(10f);
            UpdateFloorDisplay();
        }
        longElevatorAudio.Stop();
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

    private IEnumerator FadeToBlackAndLoadScene(string sceneName)
    {
        if (fadeImage == null)
        {
            yield break;
        }

        float elapsedTime = 0f;
        Color startColor = new Color(0, 0, 0, 0);
        Color endColor = new Color(0, 0, 0, 1);

        while (elapsedTime < 5f)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / 5f);
            fadeImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        fadeImage.color = endColor;

        SceneManager.LoadScene(sceneName);   
    }                                                                                                                                                     
    #endregion

    #region public functions
    public void AddScore(int amount)
    {
        score += amount;
    }
    public void SubtractScore(int amount)
    {
        score -= amount;
    }
    public void ButtonPressed(GameObject button)
    {
        button.GetComponent<SpriteRenderer>().sprite = buttonPressedSprite; 
        playButtonPressedSound();
    }
    public void ButtonReleased(GameObject button)
    {
        button.GetComponent<SpriteRenderer>().sprite = buttonSprite;
    }

    public void playTouchedSound()
    {
        basicAudio.pitch = Random.Range(0.8f, 1.2f);
        basicAudio.PlayOneShot(catTouchedClips[Random.Range(0, catTouchedClips.Length)],1f);
    }
    public void playSlapSound()
    {
        
        basicAudio.pitch = Random.Range(0.8f, 1.2f);
        basicAudio.PlayOneShot(catSlapClip[Random.Range(0, catSlapClip.Length)], 0.2f);
    }
    public void playGhostOutSound()
    {
        basicAudio.pitch = Random.Range(0.8f, 1.2f);
        basicAudio.PlayOneShot(ghostOutClip,.2f);
    }
    public void playButtonPressedSound()
    {
        basicAudio.PlayOneShot(buttonPressedClip,1f);
    }
    #endregion

    #region floor display
    private void UpdateFloorDisplay()
    {
            currentFloor -= floorsPerChange;
            targetFloor = currentFloor;
            StartCoroutine(AnimateFloorChange());
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

    #region visual juice
    private void StartButtonGlow()
    {
        glowCoroutineLeft = GlowButton(leftButton);
        glowCoroutineRight = GlowButton(rightButton);
        StartCoroutine(glowCoroutineLeft);
        StartCoroutine(glowCoroutineRight);
    }

    private IEnumerator GlowButton(GameObject button)
    {
        // Find the sprite renderer in children
        SpriteRenderer glowSprite = button.transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (glowSprite == null) yield break;

        // Enable and set initial color
        glowSprite.gameObject.SetActive(true);
        Color originalColor = glowSprite.color;
        
        // glow effect
        while (true)
        {
            float t = Mathf.PingPong(Time.time * 2f, 1f);
            float alpha = Mathf.Lerp(0f, 0.2f, t);
            glowSprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }
    private IEnumerator ShakeElevator()
    {
        while (currentState == CurrentState.Moving)
        {
            // shake the elevator randomly
            float x = Mathf.PerlinNoise(Time.time * shakeSpeed, 0) * 2 - 1;
            float y = Mathf.PerlinNoise(0, Time.time * shakeSpeed) * 2 - 1;
            Vector3 shakeOffset = new Vector3(x, y, 0) * shakeAmount;
            elevator.transform.position = originalElevatorPosition + shakeOffset;
            yield return null;
        }
        // reset elevator position when stopped
        elevator.transform.position = originalElevatorPosition;
    }
    #endregion
}
