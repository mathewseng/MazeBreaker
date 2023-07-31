using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplatePelletScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(5, 5, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Template Pellet Collected!");

            // Current Score is tracking in Game Manager
            GameManager.playerScore += 0;

            // Call player Manager Function to trigger corresponding changes.
            other.gameObject.GetComponent<PlayerManager>().CollectTemplatePellet();

            // Currently distory the gameobject
            // if needed change to hidden and reactive by a gameobject list
            Destroy(gameObject);
        }
    }
}
