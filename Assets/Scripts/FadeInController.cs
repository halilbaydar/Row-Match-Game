using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeInController : MonoBehaviour
{
    public void Levels()
    {
        SceneManager.LoadScene("Level Select Scene");
    }
}
