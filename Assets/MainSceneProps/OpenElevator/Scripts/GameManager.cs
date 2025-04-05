using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject player;
    public GameObject leftDoor;
    public GameObject rightDoor;

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
    }

    void Start()
    {
        currentState = CurrentState.Stopped;
        score = 0;
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
        //TODO: play elevator stop sound
        DoorOpen();
        yield return new WaitForSeconds(15f);
        currentState = CurrentState.Warning;
    }
    IEnumerator StartWarning()
    {   
        //TODO: play warning sound and light
        DoorClose();
        yield return new WaitForSeconds(2f);
        currentState = CurrentState.Moving;
    }
    void StartMoving()
    {
        //TODO: play elevator start moving sound
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
