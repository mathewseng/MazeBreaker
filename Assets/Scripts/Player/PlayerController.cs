using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player Movement Control relevant functions.
public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed;

    [SerializeField] Vector3 movement;

    private Rigidbody rb;
    private Vector3 PlayerMovementInput;
    //private Transform cameraTransform;
    private Vector3 cameraRotation;
    Animator anim;

    GameObject player;
    PlayerManager PlayerManager;
    private bool jump;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        //cameraTransform = new GameObject().transform;
        //cameraTransform.position = Camera.main.transform.position;
        //cameraTransform.rotation = Quaternion.LookRotation(Vector3.forward);

        cameraRotation = Vector3.forward;

        Physics.gravity = new Vector3(0, -100f, 0);
        
        player = GameObject.FindGameObjectWithTag("Player");
        PlayerManager = player.GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        InputUpdate();
        PlayerMove();
        anim.SetFloat("vertical", GetComponent<Rigidbody>().velocity.magnitude);

        // Check if the PacMan has moved off the left side of the screen
        if (transform.position.x <= -10)
        {
            // Teleport the PacMan to the right side of the screen
            transform.position = new Vector3(240, transform.position.y, transform.position.z);
        }
        // Check if the PacMan has moved off the right side of the screen
        else if (transform.position.x >= 250)
        {
            // Teleport the PacMan to the left side of the screen
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
        }
    }

    void InputUpdate()
    {
        movement = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        jump = Input.GetKeyDown(KeyCode.Space) && rb.position.y < 0.1f;
    }

    /*void PlayerMove()
    {
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        
        // If 'Q' key is pressed, rotate the vectors anti-clockwise (to the left)
        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            cameraForward = Quaternion.Euler(0, -90, 0) * cameraForward;
            cameraRight = Quaternion.Euler(0, -90, 0) * cameraRight;
        }
        // If 'E' key is pressed, rotate the vectors clockwise (to the right)
        else if (Input.GetKeyDown(KeyCode.E))
        {
            cameraForward = Quaternion.Euler(0, 90, 0) * cameraForward;
            cameraRight = Quaternion.Euler(0, 90, 0) * cameraRight;
        }
        
        // Ignore pitch and normalize to get direction
        cameraForward.y = 0;
        cameraForward.Normalize();
        cameraRight.y = 0;
        cameraRight.Normalize();

        Vector3 moveDirection = cameraForward * movement.z + cameraRight * movement.x;
        rb.velocity = speed * moveDirection;
    }*/

    void PlayerMove()
    {
        FollowPlayer followPlayer = FindObjectOfType<FollowPlayer>();
        float direction = followPlayer.direction;

        Vector3 moveDirection = Vector3.zero;

        switch (direction)
        {
            case 0: // North
                moveDirection = new Vector3(movement.x, 0, movement.z);
                break;
            case 90: // East
                moveDirection = new Vector3(movement.z, 0, -movement.x);
                break;
            case 180: // South
                moveDirection = new Vector3(-movement.x, 0, -movement.z);
                break;
            case 270: // West
                moveDirection = new Vector3(-movement.z, 0, movement.x);
                break;
        }

        float yVel = rb.velocity.y;
        if (PlayerManager.status == PlayerManager.PlayerStatus.PowerUp)
        {
            if (rb.position.y < 0.1f)
            {
                rb.velocity = speed * moveDirection * 1.3f;
            }
            else
            {
                rb.velocity = speed * moveDirection * 1.43f;
            }
        }
        else {
            if (rb.position.y < 0.1f)
            {
                rb.velocity = speed * moveDirection;
            }
            else
            {
                rb.velocity = speed * moveDirection * 1.1f;
            }
        }
        rb.velocity = new Vector3(rb.velocity.x, yVel, rb.velocity.z);
        if (jump) {
            rb.AddForce(Vector3.up * 40, ForceMode.Impulse);
        }

        // Rotate the player in the direction of movement
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10);
        }
    }
}
