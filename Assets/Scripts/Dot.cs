using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    private int column;
    private int row;
    private int targetX;
    private int targetY;
    public bool isMatched = false;
    private bool scoreUpdated = false;

    private bool isDone = false;
    private GameObject otherDot;
    public GameObject greenTick;
    private ScoreManager scoreManager;

    private Board board;
    private Vector2 initialTouchPosition;
    private Vector2 finalTouchPosition;
    private float swipeAngle;
    private BoardStateMachine BoardStateMachine;
  
    // Start is called before the first frame update
    void Start()
    {
        BoardStateMachine = FindObjectOfType<BoardStateMachine>();
        scoreManager = FindObjectOfType<ScoreManager>();
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        row = targetY;
        column = targetX;
        FindMatches();
    }

    // Update is called once per frame
    void Update()
    {
        targetX = column;
        targetY = row;
        MoveDots();
    }

    private void LateUpdate()
    {
        FindMatches();
        ChangeColorIfMatchOccurred();
        UpdateScore();
    }

    private void UpdateScore()
    {
        if (this.isMatched && !scoreUpdated)
        {
            scoreManager.UpdateScoreBoard(this);
            scoreUpdated = true;
        }
    }

    private void ChangeColorIfMatchOccurred()
    {
        if (this.isDone) return;
        if (this.isMatched)
        {
            if(BoardStateMachine.GetCurrentState().StateName.Equals(State.STOP))
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.color = new Color(0f, 1f, 0f, .01f);
                GameObject tick = Instantiate(greenTick, transform.position, Quaternion.identity);
                tick.transform.parent = this.transform;
                isDone = true;
            }
        }
    }

    private void MoveDots()
    {
        //if (this.isMatched) return;
        if (Mathf.Abs(transform.position.x - targetX) > .1)
        {
            transform.position = Vector2.Lerp(transform.position, new Vector2(targetX, transform.position.y), .3f);
        }
        else
        {
            transform.position = new Vector2(targetX, transform.position.y);
            board.allDots[column, row] = this.gameObject;
        }
        if (Mathf.Abs(transform.position.y - targetY) > .1)
        {
            transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x, targetY), .3f);
        }
        else
        {
            transform.position = new Vector2(transform.position.x, targetY);
            board.allDots[column, row] = this.gameObject;
        }
    }

    private void OnMouseDown()
    {
        if (this.isMatched) return;
        if(BoardStateMachine == null)
        {
            Debug.Log("BoardStateMachine");
        }else if(BoardStateMachine.GetCurrentState() == null)
        {
            Debug.Log("BoardStateMachine.GetCurrentState()");

        }
        if (BoardStateMachine.GetCurrentState().StateName.Equals(State.MOVE)) return;

        BoardStateMachine.ChangeState(BoardStateMachine.moveState);


        initialTouchPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        if (this.isMatched) return;
        if (BoardStateMachine.GetCurrentState().StateName.Equals(State.STOP)) return;

        finalTouchPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        CalculateAngle();
        BoardStateMachine.ChangeState(BoardStateMachine.stopState);
    }

    private void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - initialTouchPosition.y , finalTouchPosition.x - initialTouchPosition.x) * 180 / Mathf.PI;
        ChangeDotPositionsOnBoard();
    }

    private void ChangeDotPositionsOnBoard()
    {
        if (SwapeAngleUp())
        {
            otherDot = board.allDots[column, row + 1];
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }
        else if (SwapeAngleDown())
        {
            otherDot = board.allDots[column, row - 1];
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;

        }
        else if (SwapeAngleLeft())
        {
            otherDot = board.allDots[column - 1, row];
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;

        }
        else if (SwapeAngleRight())
        {
            otherDot = board.allDots[column + 1, row];
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;
        }
    }

    private bool SwapeAngleUp()
    {
        return swipeAngle > 45 && swipeAngle <= 135 && row < board.Width -1 && !board.allDots[column, row + 1].GetComponent<Dot>().isMatched;
    }
    private bool SwapeAngleRight()
    {
        return swipeAngle > -45 && swipeAngle <= 45 && column < board.Width -1 && !board.allDots[column + 1, row].GetComponent<Dot>().isMatched;
    }
    private bool SwapeAngleLeft()
    {
        return (swipeAngle <= -135 || swipeAngle > 135) && column > 0 && !board.allDots[column - 1, row].GetComponent<Dot>().isMatched;
    }
    private bool SwapeAngleDown()
    {
        return swipeAngle >= -135 && swipeAngle < -45 && row > 0 && !board.allDots[column, row - 1].GetComponent<Dot>().isMatched;
    }


    private void FindMatches()
    {
        bool rowIsMatched = true;

        for (int columnIndex = 0; columnIndex < board.Width ; columnIndex++)
        {
            if (board.allDots[columnIndex, row].gameObject.tag != this.gameObject.tag)
            {
                rowIsMatched = false;
                break;
            }
        }

        if (rowIsMatched)
        {
            for (int columnIndex = 0; columnIndex < board.Width ; columnIndex++)
            {
                board.allDots[columnIndex, row].GetComponent<Dot>().isMatched = true;
            }
        }
    }
}
