using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    // Camera reference
    private Camera _camera;

    // Reference to SpawnWalls
    private SpawnWalls _spawnWalls;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _spawnWalls = FindObjectOfType<SpawnWalls>();
    }

    private void Start()
    {
        // Assume SpawnWalls script has public methods to get map width and height
        float mapWidth = _spawnWalls.GetMapWidth();
        float mapHeight = _spawnWalls.GetMapHeight();

        // Position camera at the center and above the map
        _camera.transform.position = new Vector3(mapWidth / 2.0f, mapHeight, mapHeight / 2.0f);

        // Rotate the camera to look downwards
        _camera.transform.rotation = Quaternion.Euler(90, 0, 0);

        // Adjust the camera's FOV or clipping planes if necessary
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
