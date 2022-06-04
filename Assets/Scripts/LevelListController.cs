using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelListController : MonoBehaviour
{
    public GameObject prefab;
    private List<GameObject> levelItem;
    private List<LevelInfo> levelInfos;
    public Sprite activeSprite;
    public Sprite passiveSprite;

    public static LevelListController SINGLETON;

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
    private void Start()
    {
        levelItem = new List<GameObject>();
        levelInfos = new List<LevelInfo>();
        PopulateLevelList(RowMatchDataManager.SINGLETON.LevelsData.levelInfos);
    }

    private void PopulateLevelList(LevelInfo[] levelInfos)
    {
        for (int i = 0; i < levelInfos.Length; i++)
        {
            LevelInfo levelInfo = GetLevelInfo(GetLevelFromIndex(i));
            if (levelInfo == null) throw new Exception("LEVEL NOT FOUND!!!");

            GameObject levelSelectTempItem = Instantiate(prefab, this.transform);
            //levelItem.Add(levelSelectTempItem);
            LevelListItem levelSelect = levelSelectTempItem.GetComponent<LevelListItem>();
            levelSelect.Level.text = "Level " + levelInfo.LevelNumber.ToString() + "-" + levelInfo.NumberOfMoves.ToString() + " MV";

            if (levelInfo.HighestScore != 0)
            {
                levelSelect.HighScore.text = "Highest Score : " + levelInfo.HighestScore;
            }
            else levelSelect.HighScore.text = "No Score";

            levelSelect.LevelInfo = levelInfo;
            levelSelect.PLay.image.sprite = activeSprite;

            if (i > 0)
            {
                if (GetLevelInfo(GetLevelFromIndex(i)).HighestScore == 0)
                {
                    levelSelect.PLay.enabled = false;
                    levelSelect.PLay.image.sprite = passiveSprite;
                }
            }
            levelSelect.PLay.onClick.AddListener(levelSelect.ClickPlayButton);
        }
    }

    private int GetLevelFromIndex(int i)
    {
        return i + 1;
    }

    public LevelInfo GetLevelInfo(int i)
    {
        if (i < 0) throw new Exception("INVALID_LEVEL");
        foreach(LevelInfo level in RowMatchDataManager.SINGLETON.LevelsData.levelInfos)
        {
            if(level.LevelNumber == i)
            {
                return level;
            }
        }
        return null;
    }
}
