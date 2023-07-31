using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

// Game Manager for trakcing Player Score and changes between Stages

public class GameManager : MonoBehaviour
{
    // Set the Script for each stage for accurate enemy tracking.
    // Commented out because this variable is not used and is causing a warning
    //[SerializeField] int currentStage = 0;

    public static int playerScore;
    public static int coinsCollected;
    public static int powerPelletsCollected;

    // Game Unit Object tracker list
    GameObject player;
    public GameObject[] ghosts;

    // Collectable Pellets to detect level progress.
    // End Game or Change Stage when size = 0 if pellets are destory()
    private GameObject[] pellets;

    // Store the Level Scenes.
    public Scene[] SceneList;

    public static GameManager instance = null;

    //public GameObject pacmanInstance;  // this is your current PacMan instance

    // Counter for collected pellets
    public RectTransform panel;
    public RectTransform scoreContainer;
    public TextMeshProUGUI scoreText;

    // Total # of Coins
    public int totalCoins;
    // Total # of Power Pellets
    public int totalPowerPellets;

    public static float startTime;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log("Game Manager Intialized");
        if (SceneManager.GetActiveScene().name == "level_1_zheng") {
            Time.timeScale = 1f;
            coinsCollected = 0;
            powerPelletsCollected = 0;
            playerScore = 0;
            PlayerManager.remainHP = 2;
        }

        //Screen.SetResolution(1280, 720, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        panel.sizeDelta = new Vector2(Screen.width, Screen.height);
        scoreContainer.sizeDelta = new Vector2(Screen.width, Screen.height - 15f);
        scoreText.fontSize = Screen.width / 40f;
        scoreText.text =
            " Time: " + (Time.time - startTime).ToString("0.00s") + "\n" +
            " Lives: " + (PlayerManager.remainHP + 1) + "\n" +
            " Stars: " + powerPelletsCollected + "/" + totalPowerPellets + "\n" +
            " Coins: " + coinsCollected + "/" + totalCoins + "\n" +
            " Score: " + playerScore;

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Scene Reloaded");
            SceneManager.LoadScene(SceneManager.GetActiveScene().ToString());
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                    Application.Quit();
            #endif
        }

        // When there is no pellets remains, the player finish the game.
        // if (pellets.Length == 0) endStage();
    }

    void endStage()
    {
        // State where enemy no longer respawn on the map.
        // Guiding player moving to next level.
        foreach (GameObject obj in ghosts)
        {
            Destroy(obj);
            // Call Guidlines, animation and hints helpers.
        }
    }

    // Change the Scene to the target Scene by Index.
    public void ChangeStage(int level)
    {
        // Load the target index Scene from Scene List
        SceneManager.LoadScene(SceneList[level].ToString());
    }
}
