using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    private Board board;
    private float aspectRatio = 0.5625f; //9:16
    public float cameraOffset;
    public float paddding = 2;
    private float yOffset = 1;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        if(board != null)
        {
            RepositionCamera(board.Width - 1, board.Height - 1);
        }
    }

    private void RepositionCamera(float x, float y)
    {
        this.transform.position = new Vector3(x/2, y/2 + yOffset, cameraOffset);
        if(board.Width >= board.Height)
        {
            Camera.main.orthographicSize = (board.Width / 2 + paddding) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = board.Height / 2 + paddding;
        }
    }
}
