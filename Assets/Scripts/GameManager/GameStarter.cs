using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour
{
    public string LevelName;
    public void StartGame()
    {
        SceneManager.LoadScene(LevelName);
        Time.timeScale = 1f;
        GameManager.startTime = Time.time;
        Debug.Log("continue");
    }


}
