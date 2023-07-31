using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndText : MonoBehaviour
{
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = 0f;
        if (SceneManager.GetActiveScene().name == "loseScreen")
        {
            GameObject.FindWithTag("EndText").GetComponent<TextMeshProUGUI>().text = 
                "Time: " + (Time.time - GameManager.startTime).ToString("0.00s") + "\n" +
                "Score: " + GameManager.playerScore + "\n" +
                "Venture into the maze again?";
        }
        else if (SceneManager.GetActiveScene().name == "winScreen")
        {
            GameObject.FindWithTag("EndText").GetComponent<TextMeshProUGUI>().text = 
                "Time: " + (Time.time - GameManager.startTime).ToString("0.00s") + "\n" +
                "Lives: " + (PlayerManager.remainHP + 1) + "\n" + 
                "Score: " + GameManager.playerScore + "\n" +
                "Venture into the maze again?";
        }
    }
}
