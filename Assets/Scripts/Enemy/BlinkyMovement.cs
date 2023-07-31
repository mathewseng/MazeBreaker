using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

//Enemy Movement relevant Functions.

public class BlinkyMovement : MonoBehaviour
{
    [SerializeField] float health, maxHealth = 1;

    private Rigidbody rb;
    private NavMeshAgent navMesh;

    private GameObject player;
    private PlayerManager pm;
    private SpawnWalls map;

    public Vector3 currRandomPos;
    private float chasingTime;

    public float chasingCD;
    public float restCD;

    private FollowPlayer _followPlayer;
    private GameManager gameManager;
    
    private AudioManager audioManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        navMesh = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        pm = player.GetComponent<PlayerManager>();
        map = GameObject.FindGameObjectWithTag("GameManager").GetComponent<SpawnWalls>();
        currRandomPos = map.freeSpaces[Random.Range(0, map.freeSpaces.Count)];
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Reset to make sure the enemy rechase the player.
        if (chasingTime > 27) chasingTime = 0;

        switch (pm.getStatus())
        {
            case 0:// Normal Player
                if (Vector3.Distance(transform.position, player.transform.position) < 100 && chasingTime < chasingCD)
                {
                    navMesh.SetDestination(player.transform.position);
                }
                else
                {
                    // Random Wander Around if far
                    // Enemy Wander Around.

                    if (Vector3.Distance (this.transform.position, currRandomPos) <= 50)
                    {
                        currRandomPos = map.freeSpaces[Random.Range(0, map.freeSpaces.Count)];
                    }

                    navMesh.SetDestination(currRandomPos);



                    // Reset chasing counter after fixed time duration.
                    if (chasingTime > chasingCD + restCD) chasingTime = 0;
                }

                if (this.gameObject.transform.localScale.x < 3.5f)
                {
                    this.gameObject.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
                }
                 

                break;
            case 1:// PowerUp Player
                if (Vector3.Distance(transform.position, player.transform.position) < 50)
                {
                    if (Vector3.Distance(this.transform.position, currRandomPos) <= 120)
                    {
                        currRandomPos = map.freeSpaces[Random.Range(0, map.freeSpaces.Count)];
                    }
                    navMesh.SetDestination(currRandomPos);
                }

                if (this.gameObject.transform.localScale.x > 2.5f)
                {
                    this.gameObject.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                }

                break;
        }

    }

    private void FixedUpdate()
    {
        chasingTime += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (pm.getStatus())
            {
                case 0: // Normal
                    // Call Corresponding Function in player when touched by enemy.
                    player.GetComponent<PlayerManager>().HitByEnemey(); // Teleport Player GameObject to Initial Place.
                    Debug.Log("Blinky Killed You!");
                    break;
                case 1: // PowerUp
                    // The Ghost will be distoryied, and generate another enemy in the spawn point.
                    GameManager.playerScore += 1000;
                    Debug.Log("Blinky Dead!");
                    gameManager.GetComponent<SpawnWalls>().SpawnBlinky();
                    Destroy(this.gameObject);
                    audioManager.PlayEatEnemySound();
                    break;
            }
        }
    }
}
