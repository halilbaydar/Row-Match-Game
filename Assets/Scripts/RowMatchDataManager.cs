using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using System.Collections.Generic;
using System.Net;

[Serializable]
public class LevelsData
{
    public LevelInfo[] levelInfos;
}

//TODO This is not an optimal solution beacuse as the data grows this process will become very slow
//TODO In an enterprise application db should be used like SQLLite to solve performance issues
public class RowMatchDataManager : MonoBehaviour
{
    private static readonly string GOOGLE = "http://google.com";
    private static readonly string LEVEL_PATH = "Assets/Levels/";
    private static readonly string LEVEL_BASE_URL = "https://row-match.s3.amazonaws.com/levels/RM_";

    public LevelsData LevelsData;
    public LevelsData TempLevelsData;

    public static RowMatchDataManager SINGLETON;

    private void Awake()
    {
        if(SINGLETON == null)
        {
            SINGLETON = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        ManageLevelData();
        StartCoroutine(PullLevelDataFromRemote());
        StartCoroutine(ManageRemoteLevels());
        Debug.Log("RowMatchDataManager.SINGLETON.LevelsData: " + SINGLETON.LevelsData.levelInfos.Length);
    }

    private IEnumerator ManageRemoteLevels()
    {
        while (true)
        {
            if (LevelFilesDownloaded())
            {
                LoadRawLevelsFromDisk();
                IsLevelDataVersionUpdated((updated) =>
                {
                    if (!updated)
                    {
                        UpdateLevelArray();
                        SaveLevelData();
                    }
                });
                StopCoroutine(ManageRemoteLevels());
            }
            yield return new WaitForSeconds(180);
        }
    }

    private void UpdateLevelArray()
    {
        List<LevelInfo> levelInfos = new List<LevelInfo>();
        if (this.LevelsData == null) return;
        foreach(LevelInfo level in this.LevelsData.levelInfos)
        {
            levelInfos.Add(level);
        }
        if(this.TempLevelsData != null)
        {
            foreach (LevelInfo level in this.TempLevelsData.levelInfos)
            {
                bool exists = false;
                foreach(LevelInfo level1 in this.LevelsData.levelInfos)
                {
                    if(level1.LevelNumber == level.LevelNumber)
                    {
                        exists = true;
                        break;
                    }
                }
                if(!exists) levelInfos.Add(level);
            }
        }
        this.LevelsData.levelInfos = levelInfos.ToArray();
    }

    private void ManageLevelData()
    {
        LevelFileExists((fileExists) => {
            if (!fileExists)
            {
                LoadRawLevelsFromDisk();
                this.LevelsData.levelInfos = this.TempLevelsData.levelInfos;
                this.TempLevelsData = null;
                //SaveLevelData();
            }
            else
            {
                LoadLevelDataFromDisk();
            }
        });
    }

    private void LevelFileExists(Action<bool> action)
    {
        StartCoroutine(FileExists((fileExists) => {
            if (fileExists)
            {
                //Create binary formatter which will read binary files from disk
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                //Create a route from the application to file on disk
                FileStream file = File.Open(Application.persistentDataPath + "/levels.dat", FileMode.Open);
                this.TempLevelsData = binaryFormatter.Deserialize(file) as LevelsData;
                action(this.TempLevelsData.levelInfos.Length > 0);
                file.Close();
            }
        }, Application.persistentDataPath + "/levels.dat"));
    }

    private bool LevelFilesDownloaded()
    {
        int count = 0;
        int i = 0;
        string[] fileNames = Directory.GetFiles(LEVEL_PATH);
        for (; i < fileNames.Length; i++)
        {
            if (!fileNames[i].Contains(".meta"))
            {
                count++;
            }
        }
        Debug.Log("Level number: " + count);
        return count == 25;
    }

    public void OnDisable()
    {
        SaveLevelData();
    }

    //This method downloads remote level files
    public IEnumerator PullLevelDataFromRemote()
    {
        WebClient mywebClient = new WebClient();
        while (true)
        {
            StartCoroutine(CheckInternetConnection((isConnected) =>
            {
                Debug.Log("CONNECTED: " + isConnected);
                if (isConnected)
                {
                    for (int i = 11; i < 26; i++)
                    {
                        string filename = GetFileName(i);
                        StartCoroutine(FileExists((fileExists) =>
                        {
                            if (!fileExists)
                            {
                                string URL = LEVEL_BASE_URL + filename;
                                mywebClient.DownloadFile(URL, LEVEL_PATH + filename);
                            }
                        }, LEVEL_PATH + "RM_" + filename));
                    }
                    StopCoroutine(PullLevelDataFromRemote());
                }
            }));
            yield return new WaitForSeconds(180);
        }
    }

    IEnumerator CheckInternetConnection(Action<bool> action)
    {
        WWW www = new WWW(GOOGLE);
        yield return www;
        action(www.error == null);
    }

    IEnumerator FileExists(Action<bool> fileExists, string path)
    {
        yield return new WaitForSeconds(10);
        fileExists(File.Exists(path));
    }

    private string GetFileName(int i)
    {
        if(i > 15)
        {
            return "B" + i % 15;
        }
        return "A" + i;
    }

    public static bool LevelExists(int level)
    {
        return false;
    }

    public void LoadLevelDataFromDisk()
    {
        Debug.Log("LoadLevelDataFromDisk");
        //Check file exists
        StartCoroutine(FileExists((fileExists) => {
            Debug.Log("fileExists: " + fileExists);
            if (fileExists)
            {
                //Create binary formatter which will read binary files from disk
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                //Create a route from the application to file on disk
                FileStream file = File.Open(Application.persistentDataPath + "/levels.dat", FileMode.Open);
                this.LevelsData = binaryFormatter.Deserialize(file) as LevelsData;
                file.Close();
            }
        }, Application.persistentDataPath + "/levels.dat"));
    }

    public void SaveLevelData()
    {
        //Create binary formatter which will read binary files from disk
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        //Create a route from the application to file on disk
        FileStream file = File.Open(Application.persistentDataPath + "/levels.dat", FileMode.OpenOrCreate);
        LevelsData tempLevelsData = new LevelsData();
        tempLevelsData = this.LevelsData;
        //Serialize the the given class
        binaryFormatter.Serialize(file, tempLevelsData);
        //Close data stream
        file.Close();
    }

    private void IsLevelDataVersionUpdated(Action<bool> updated)
    {
        updated(this.TempLevelsData.levelInfos.Length == this.LevelsData.levelInfos.Length);
    }

    //Load raw files from disk to this.TempLevelsData.levelInfos
    private void LoadRawLevelsFromDisk()
    {
        List<LevelInfo> levelInfos = new List<LevelInfo>();

        if (Directory.Exists(LEVEL_PATH))
        {
            int i = 0;
            string[] fileNames = Directory.GetFiles(LEVEL_PATH);
            for (; i < fileNames.Length; i++)
            {
                if (!fileNames[i].Contains(".meta"))
                {
                    if (File.Exists(fileNames[i]))
                    {
                        string text = File.ReadAllText(fileNames[i]);
                        string[] lines = text.Split('\n');

                        int level_number = 0;
                        int grid_width = 0;
                        int grid_height = 0;
                        int move_count = 0;
                        string[] grid = { };

                        for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
                        {
                            if (lineNumber == 0)
                            {
                                level_number = Int16.Parse(lines[lineNumber].Split(':')[1]);
                            }
                            else if (lineNumber == 1)
                            {
                                grid_width = Int16.Parse(lines[lineNumber].Split(':')[1]);
                            }
                            else if (lineNumber == 2)
                            {
                                grid_height = Int16.Parse(lines[lineNumber].Split(':')[1]);
                            }
                            else if (lineNumber == 3)
                            {
                                move_count = Int16.Parse(lines[lineNumber].Split(':')[1]);
                            }
                            else if (lineNumber == 4)
                            {
                                grid = lines[lineNumber].Split(':')[1].Split(',');
                            }
                        }
                        LevelInfo levelInfo = new LevelInfo();
                        levelInfo.Dots = grid;
                        levelInfo.HighestScore = 0;
                        levelInfo.Width = grid_width;
                        levelInfo.Height = grid_height;
                        levelInfo.NumberOfMoves = move_count;
                        levelInfo.LevelNumber = level_number;
                        levelInfos.Add(levelInfo);
                    }
                }
            }
        }
        this.TempLevelsData.levelInfos = levelInfos.ToArray();
    }
}
