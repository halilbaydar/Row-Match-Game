using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private int width;
    private int height;
    private int numberOfMoves;

    public int Width
    {
        get { return width; }
        set { width = value; }
    }

    public int NumberOfMoves
    {
        get { return numberOfMoves; }
        set { numberOfMoves = value; }
    }


    public int Height
    {
        get { return height; }
        set { height = value; }
    }

    public GameObject tilePrefab;
    public GameObject[] dots;
    public GameObject[,] allDots;
    private int level;

    internal void ApplyEndGame()
    {
        StartCoroutine(GameEndded());
    }

    public IEnumerator GameEndded()
    {
        yield return null;
    }


    void Awake()
    {
        level = PlayerPrefs.GetInt("Level");
    }

    // Start is called before the first frame update
    void Start()
    {

        LevelInfo levelInfo = LevelListController.SINGLETON.GetLevelInfo(level);

        width = levelInfo.Width;
        height = levelInfo.Height;
        NumberOfMoves = levelInfo.NumberOfMoves;
        string[] dotNames = levelInfo.Dots;

        allDots = new GameObject[width, height];

        SetUpBoard(width,height,dotNames);
    }

    private void SetUpBoard(int width, int height, string[] dotNames)
    {
        int index = 0;
        for(int i=0; i<width; i++)
        {
            for(int j=0; j<height; j++)
            {
                Vector2 position = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefab, position, Quaternion.identity);
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";

                GameObject dot = Instantiate(GetDot(dotNames[index]), position, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = backgroundTile.name;
                
                allDots[i,j] = dot;
                index++;
            }
        }
    }

    private GameObject GetDot(string v)
    {
        //for(int i=0; i<dots.Length; i++)
        //{
        //    if (dots[i].tag.ToString().Equals(v))
        //    {
        //        return dots[i];
        //    }
        //}
        //throw new System.Exception();
        return dots[Random.Range(0,dots.Length-1)];
    }

    public void DecreaseRemainMoves()
    {
        this.NumberOfMoves -= -1;
    }
}
