using UnityEngine;
using System.Collections;

public class BoardStateMachine : MonoBehaviour
{
    private BaseState currentState;
    public MoveState moveState;
    public StopState stopState;
    public Board board;

    private void Awake()
    {
        board = FindObjectOfType<Board>();
        moveState = new MoveState(this);
        stopState = new StopState(this);
    }

    // Use this for initialization
    void Start()
    {
        if (currentState == null)
            currentState = GetInitialState();
        currentState.Enter();
    }

    public BaseState GetInitialState()
    {
        return stopState;
    }

    public BaseState GetCurrentState()
    {
        return currentState;
    }

    public void ChangeState(BaseState newState)
    {
        currentState.Exit();

        currentState = newState;

        currentState.Enter();
    }
}
