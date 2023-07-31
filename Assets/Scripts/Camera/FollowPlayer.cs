using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public float direction = 0; // The direction the camera is facing
    private Vector3 offset;
    private float rotationSpeed = 0.25f; // Change duration of rotation
    private float currentAngle = 0; // Current angle of the camera
    private bool isRotating = false;

    // Use this for initialization
    void Start()
    {
        CalculateOffset(60);
        transform.rotation = Quaternion.Euler(30, currentAngle, 0);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //CalculateOffset(60);
        // Here we adjust the camera's position to follow the player
        transform.position = player.position + Quaternion.Euler(0, currentAngle, 0) * offset;

        // Rotate camera
        if (Input.GetKeyDown(KeyCode.Q) && !isRotating)
        {
            StartCoroutine(RotateCamera(Vector3.up, -90, rotationSpeed));
        }
        if (Input.GetKeyDown(KeyCode.E) && !isRotating)
        {
            StartCoroutine(RotateCamera(Vector3.up, 90, rotationSpeed));
        }
        if (Input.GetKeyDown(KeyCode.R) && !isRotating)
        {
            StartCoroutine(RotateCamera(Vector3.up, 180, rotationSpeed));
        }
    }

    IEnumerator RotateCamera(Vector3 axis, float angle, float duration = 1.0f)
    {
        isRotating = true;
        float from = currentAngle;
        float to = currentAngle + angle;

        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            currentAngle = Mathf.Lerp(from, to, elapsed / duration);
            transform.rotation = Quaternion.Euler(30, currentAngle, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        currentAngle = to;
        transform.rotation = Quaternion.Euler(30, currentAngle, 0);
        direction = ((int)currentAngle % 360 + 360) % 360;
        isRotating = false;
    }

    private void CalculateOffset(float angle)
    {
        float radian = angle * Mathf.Deg2Rad; // Convert the angle to radian
        offset = new Vector3(0, 10, -7 * Mathf.Sin(radian));
    }
}
