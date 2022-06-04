using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelListItem : MonoBehaviour
{
    [SerializeField]
    private Text level;
    [SerializeField]
    private Text highScore;
    [SerializeField]
    private Button play;
    private LevelInfo levelInfo;

    public Button PLay
    {
        get { return play; }
        set { play = value; }
    }

    public Text Level
    {
        get { return level; }
        set { level = value; }
    }

    public Text HighScore
    {
        get { return highScore; }
        set { highScore = value; }
    }

    public LevelInfo LevelInfo
    {
        get { return levelInfo; }
        set { levelInfo = value; }
    }

    private void Start()
    {
        
    }
    public void ClickPlayButton()
    {
        PlayerPrefs.SetInt("Level", levelInfo.LevelNumber);
        SceneManager.LoadScene("Main Scene");
    }
}
