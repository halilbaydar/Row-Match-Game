using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;
    private int score;
    private static readonly object syncLock = new object();

    //[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
    public void UpdateScoreBoard(Dot dot)
    {
        int score = 0;
        if (dot.tag == "g")
        {
            score = 150;
        }
        else if (dot.tag == "y")
        {
            score = 250;
        }
        else if (dot.tag == "r")
        {
            score = 100;
        }
        else if (dot.tag == "b")
        {
            score = 200;
        }
        UpdateScore(score);
    }

    private void UpdateScore(int value)
    {
        lock (syncLock)
        {
            score += value;
            scoreText.text = score.ToString();
        }
    }
}
