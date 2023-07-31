using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPelletScript : MonoBehaviour
{
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Animation to the Collectable Object
        transform.Rotate(0, 2, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Power Pellet Collected!");

            // Current Score is tracking in Game Manager
            GameManager.playerScore += 500;
            GameManager.powerPelletsCollected += 1;

            other.gameObject.GetComponent<PlayerManager>().CollectPowerPellet();
            
            // Play star collect sound
            audioManager.PlayStarCollectSound();

            // Currently distory the gameobject
            // if needed change to hidden and reactive by a gameobject list
            Destroy(gameObject);
        }
    }
}
