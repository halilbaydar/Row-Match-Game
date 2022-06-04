using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameManager : MonoBehaviour
{
    private Board board;
    public static EndGameManager SINGLETON;

    private void Awake()
    {
        if (SINGLETON == null)
        {
            SINGLETON = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FinishTheGame()
    {
        //board.allDots[columnIndex, row].GetComponent<Dot>().isMatched

        if(board.NumberOfMoves == 0)
        {
            DoFinish();
            return;
        }

        //int startRow = 0;
        //int endRow = SINGLETON.board.Height - 1;

        //for (int row = 0; row < SINGLETON.board.Height - 1;)
        //{
        //    if (!SINGLETON.board.allDots[0, row].GetComponent<Dot>().isMatched)
        //    {
        //        startRow = row;
        //        for (int row1 = row + 1; row1 < SINGLETON.board.Height - 1; row1++)
        //        {
        //            Debug.Log("row1: " + row1);

        //            if (SINGLETON.board.allDots[0, row1].GetComponent<Dot>().isMatched)
        //            {
        //                Debug.Log("row1: " + row1);
        //                endRow = row1;
        //                break;
        //            }
        //        }
        //        Debug.Log("startRow: " + startRow + " endRow: " + endRow);

        //        //bool isExists = IsThereAPossibleMathcInThisRange(startRow, endRow);
        //        //if (isExists) return;
        //        startRow = endRow;
        //        row = endRow;
        //        continue;
        //    }
        //    row++;
        //}

        //DoFinish();
    }

    private bool IsThereAPossibleMathcInThisRange(int startRow, int endRow)
    {
        int height = SINGLETON.board.Height;
        int width = SINGLETON.board.Width;
        int b = 0, g = 0, y = 0, r = 0;

        Debug.Log("startRow: " + startRow + " endRow: " + endRow);

        for (; startRow < endRow - 1; startRow++)
        {
            for (int tempHeight = 0; tempHeight < SINGLETON.board.Height - 1; tempHeight++)
            {
                if (board.allDots[tempHeight, startRow].GetComponent<Dot>().isMatched) continue;

                if(board.allDots[tempHeight, startRow].GetComponent<Dot>().tag == "b")
                {
                    b++;
                }
                else if (board.allDots[tempHeight, startRow].GetComponent<Dot>().tag == "r")
                {
                    r++;
                }
                else if (board.allDots[tempHeight, startRow].GetComponent<Dot>().tag == "g")
                {
                    g++;
                }
                else if (board.allDots[tempHeight, startRow].GetComponent<Dot>().tag == "y")
                {
                    y++;
                }
            }
        }
        return b == width || g == width || y == width || r == width;
    }

    private void DoFinish()
    {
        //Debug.Log("GAME FINISHED");
    }
}
