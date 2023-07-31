using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PelletScript : MonoBehaviour
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
        transform.Rotate(0, 1, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Pellet Player Collected!");

            // Current Score is tracking in Game Manager
            GameManager.playerScore += 100;
            GameManager.coinsCollected += 1;

            // Play coin collect sound
            audioManager.PlayCoinCollectSound();

            // Currently distory the gameobject
            // if needed change to hidden and reactive by a gameobject list
            Destroy(gameObject);
        }
    }
}
