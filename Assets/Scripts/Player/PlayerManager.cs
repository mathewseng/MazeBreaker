using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

// Player Manager for ability and object interaction events.

public class PlayerManager : MonoBehaviour
{
    // Enum that contains all type of player power ups.
    public enum PlayerStatus
    {
        Normal,
        PowerUp
    }
    // Boolean for ghost to check player status.
    // If collect any power up, then ghost switch to escaping mode.
    public static PlayerStatus status;

    // Integer counter how many time the player could take a hit.
    public static int remainHP = 2;

    // Integer to check current Buff status duration.
    // powerUp to false when == 0.
    private float coolDown = 0;

    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInit();
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (status)
        {
            case PlayerStatus.Normal:
                break;
            case PlayerStatus.PowerUp:
                // Set player to default status when not in powerup duration.
                if (coolDown <= 0)
                {
                    AbilityClean();
                    status = PlayerStatus.Normal;
                    transform.localScale = Vector3.one * 3.5f; // Reset the scale to 1x
                }
                else if (coolDown <= 3) {
                    if (coolDown % 0.5f < 0.25f)
                    {
                        transform.localScale = Vector3.one * 3.5f; // Reset the scale to 1x
                    }
                    else
                    {
                        transform.localScale = Vector3.one * 4.2f; // Set the scale to 1.2x
                    }
                }
                else
                {
                    transform.localScale = Vector3.one * 4.2f; // Set the scale to 1.2x
                }
                break;
        }
    }

    private void FixedUpdate()
    {
        // When CoolDown is larger than zero, minus 1 per seconds.
        coolDown -= (coolDown > 0) ? 1 * Time.deltaTime : 0;

        // Debugs
        //Debug.Log(status);
    }

    //Function called when collide with enemy.
    public void HitByEnemey()
    {

        // Respawn Player at a random empty space.
        if (remainHP > 0)
        {
            remainHP--;
            this.transform.position = GameObject.FindGameObjectWithTag("GameManager").GetComponent<SpawnWalls>()._birthPlace;
            audioManager.PlayDeathSound();
        }
        else
        {
            //End Scene
            SceneManager.LoadScene("loseScreen");
            Destroy(this.gameObject);
        }
        
    }



    // PowerUp Collector Function
    // For each type of power up, there should be a corresponding ability function.
    public void CollectPowerPellet()
    {
        status = PlayerStatus.PowerUp;
        coolDown = 15;
        Debug.Log("I AM THE STORM!");
    }

    // This is an sample.
    public void CollectTemplatePellet()
    {
        Debug.Log("This is a collector Template");
    }


    // Initialize Functions
    public void PlayerInit()
    {
        status = PlayerStatus.Normal;
        coolDown = 0;
    }

    public void AbilityClean()
    {
        // Any Other Change On GameObject
    }

    // Getter
    public int getStatus()
    {
        return (int)status;
    }
}