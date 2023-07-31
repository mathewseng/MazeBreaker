using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SpawnWalls : MonoBehaviour
{
    [Header("Spawn Grids Setting")]
    [SerializeField] private int _col = 10;
    [SerializeField] private int _row = 10;
    [SerializeField] private float _xSpace = 10f;
    [SerializeField] private float _ySpace = 10f;
    [SerializeField] private GameObject _grid;
    [SerializeField] private GameObject _wall; // Assign the wall prefab to this variable
    [SerializeField] private GameObject _pacManPrefab;
    [SerializeField] private GameObject _blinkyPrefab;
    [SerializeField] private GameObject _clydePrefab;
    [SerializeField] private GameObject _inkyPrefab;
    [SerializeField] private GameObject _pinkyPrefab;
    [SerializeField] private GameObject _coinPrefab;
    [SerializeField] private GameObject _powerPrefab;
    [SerializeField] public Vector3 _birthPlace;
    [SerializeField] public Vector3 _birthPlaceBlinky;
    [SerializeField] public Vector3 _birthPlaceClyde;
    [SerializeField] public Vector3 _birthPlaceInky;
    [SerializeField] public Vector3 _birthPlacePinky;

    private FollowPlayer _cameraScript;
    [SerializeField] private RawImage minimap;

    private TimeTracker _timeTracker;

    public List<Vector3> freeSpaces = new List<Vector3>();
    private Dictionary<Vector3, GameObject> wallDict = new Dictionary<Vector3, GameObject>();

    public GameObject collectableList;
    public GameObject gridList;
    public GameObject obstacleList;
    public string WinScreen;

    //public NavMeshSurface surface;

    public enum WallSide
    {
        Left,
        Right,
        Top,
        Bottom
    }

    void Awake()
    {
        // Here you're assuming that the FollowPlayer script is attached to the Main Camera
        // Replace "Main Camera" with the name of your camera GameObject if it's different
        _cameraScript = GameObject.Find("Main Camera").GetComponent<FollowPlayer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _timeTracker = GetComponent<TimeTracker>();

        for (int i = -1; i <= _row; i++)
        {
            for (int j = -1; j <= _col; j++)
            {
                Vector3 spawnPosition = new Vector3(_xSpace * j, 0, _ySpace * i);
                Instantiate(_grid, spawnPosition, Quaternion.identity, gridList.transform);
            }
        }

        for (int i = -1; i <= _row; i++)
        {
            for (int j = -1; j <= _col; j++)
            {
                Vector3 spawnPosition = new Vector3(_xSpace * j, 0, _ySpace * i);
                freeSpaces.Add(spawnPosition);
            }
        }

        SpawnWall();
        CreateWallSpaceWithExit(17, 7, 21, 9);
        CreateWallSpace(17, 4, 21, 4);
        CreateWallSpace(1, 11, 23, 11);
        CreateWallSpace(0, 7, 1, 7);
        CreateWallSpace(0, 4, 1, 4);
        CreateWallSpace(23, 7, 24, 7);
        CreateWallSpace(23, 4, 24, 4);
        CreateWallSpace(15, 1, 23, 2);
        CreateWallSpace(23, 8, 24, 9);
        CreateWallSpaceWithExit(15, 9, 9, 4);
        CreateWallSpaceWithExitFreedom(new Vector2(9, 4), new Vector2(15, 9), WallSide.Bottom);
        CreateObstacle(new Vector2(9, 4), new Vector2(15, 9));
        CreateWallSpace(3, 8, 7, 9);
        CreateWallSpace(0, 3, 1, 3);
        CreateWallSpace(1, 1, 13, 1);
        CreateWallSpace(3, 3, 7, 3);
        SpawnPowerItem(new Vector3(0, 0, 0));
        SpawnPowerItem(new Vector3(0, 0, 120));
        SpawnPowerItem(new Vector3(240, 0, 0));
        SpawnPowerItem(new Vector3(240, 0, 120));

        //Create NavMesh after you've finished generating the walls and grids
        var navMeshData = new NavMeshData();
        NavMesh.AddNavMeshData(navMeshData);
        var sources = new List<NavMeshBuildSource>();
        foreach (var meshFilter in FindObjectsOfType<MeshFilter>())
        {
            var meshBuildSource = new NavMeshBuildSource
            {
                shape = NavMeshBuildSourceShape.Mesh,
                sourceObject = meshFilter.sharedMesh,
                transform = meshFilter.transform.localToWorldMatrix,
                area = 0
            };
            sources.Add(meshBuildSource);
        }
        var defaultBuildSettings = NavMesh.GetSettingsByID(0);
        NavMeshBuilder.UpdateNavMeshData(navMeshData, defaultBuildSettings, sources, new Bounds(Vector3.zero, new Vector3(500f, 500f, 500f)));

        var pacMan = RespawnPacMan();
        _cameraScript.player = pacMan.transform;
        RespawnBlinky();
        RespawnClyde();
        RespawnInky();
        RespawnPinky();

        SpawnCoins(freeSpaces);
    }


    void SpawnWall()
    {
        // Spawn top and bottom walls
        for (int i = 0; i <= _col; i++)
        {
            Vector3 topWallPos = new Vector3(_xSpace * i, 0, -_ySpace);
            Vector3 bottomWallPos = new Vector3(_xSpace * i, 0, _ySpace * _row);
            Instantiate(_wall, topWallPos, Quaternion.identity, obstacleList.transform);
            Instantiate(_wall, bottomWallPos, Quaternion.identity, obstacleList.transform);

            freeSpaces.Remove(topWallPos);
            freeSpaces.Remove(bottomWallPos);
        }

        // Spawn left and right walls
        for (int i = 0; i <= _row; i++)
        {
            // Leave a hole in the middle of the left and right walls
            if (i != _row / 2 && i != _row / 2 - 1)
            {
                Vector3 leftWallPos = new Vector3(-_xSpace, 0, _ySpace * i);
                Vector3 rightWallPos = new Vector3(_xSpace * _col, 0, _ySpace * i);
                Instantiate(_wall, leftWallPos, Quaternion.identity, obstacleList.transform);
                Instantiate(_wall, rightWallPos, Quaternion.identity, obstacleList.transform);

                freeSpaces.Remove(leftWallPos);
                freeSpaces.Remove(rightWallPos);
            }
        }

        // Spawn corners explicitly
        Vector3 topLeftCornerPos = new Vector3(-_xSpace, 0, -_ySpace);
        Vector3 topRightCornerPos = new Vector3(_xSpace * _col, 0, -_ySpace);
        Vector3 bottomLeftCornerPos = new Vector3(-_xSpace, 0, _ySpace * _row);
        Vector3 bottomRightCornerPos = new Vector3(_xSpace * _col, 0, _ySpace * _row);

        Instantiate(_wall, topLeftCornerPos, Quaternion.identity, obstacleList.transform);
        Instantiate(_wall, topRightCornerPos, Quaternion.identity, obstacleList.transform);
        Instantiate(_wall, bottomLeftCornerPos, Quaternion.identity, obstacleList.transform);
        Instantiate(_wall, bottomRightCornerPos, Quaternion.identity, obstacleList.transform);

        freeSpaces.Remove(topLeftCornerPos);
        freeSpaces.Remove(topRightCornerPos);
        freeSpaces.Remove(bottomLeftCornerPos);
        freeSpaces.Remove(bottomRightCornerPos);
    }

    void CreateWallSpace(int left, int bottom, int right, int top)
    {
        // Top and bottom walls
        for (int i = left; i <= right; i++)
        {
            Vector3 topWallPos = new Vector3(_xSpace * i, 0, _ySpace * top);
            Vector3 bottomWallPos = new Vector3(_xSpace * i, 0, _ySpace * bottom);
            Instantiate(_wall, topWallPos, Quaternion.identity, obstacleList.transform);
            Instantiate(_wall, bottomWallPos, Quaternion.identity, obstacleList.transform);

            freeSpaces.Remove(topWallPos);
            freeSpaces.Remove(bottomWallPos);
        }

        // Left and right walls
        for (int i = bottom + 1; i < top; i++)
        {
            Vector3 leftWallPos = new Vector3(_xSpace * left, 0, _ySpace * i);
            Vector3 rightWallPos = new Vector3(_xSpace * right, 0, _ySpace * i);
            Instantiate(_wall, leftWallPos, Quaternion.identity, obstacleList.transform);
            Instantiate(_wall, rightWallPos, Quaternion.identity, obstacleList.transform);

            freeSpaces.Remove(leftWallPos);
            freeSpaces.Remove(rightWallPos);
        }
    }

    void CreateWallSpaceWithExit(int left, int bottom, int right, int top)
    {
        int exitX = (left + right) / 2;  // X-coordinate of the exit

        // Top and bottom walls
        for (int i = left; i <= right; i++)
        {
            if (i != exitX)  // Skip creating the wall at the exit
            {
                Vector3 topWallPos = new Vector3(_xSpace * i, 0, _ySpace * top);
                Instantiate(_wall, topWallPos, Quaternion.identity, obstacleList.transform);
            }

            Vector3 bottomWallPos = new Vector3(_xSpace * i, 0, _ySpace * bottom);
            Instantiate(_wall, bottomWallPos, Quaternion.identity, obstacleList.transform);
        }

        // Left and right walls
        for (int i = bottom + 1; i < top; i++)
        {
            Vector3 leftWallPos = new Vector3(_xSpace * left, 0, _ySpace * i);
            Vector3 rightWallPos = new Vector3(_xSpace * right, 0, _ySpace * i);
            Instantiate(_wall, leftWallPos, Quaternion.identity, obstacleList.transform);
            Instantiate(_wall, rightWallPos, Quaternion.identity, obstacleList.transform);

        }

        // Create the walls
        for (int i = left; i <= right; i++)
        {
            for (int j = bottom; j <= top; j++)
            {
                Vector3 wallPos = new Vector3(_xSpace * i, 0, _ySpace * j);
                //GameObject wall = Instantiate(_wall, wallPos, Quaternion.identity, transform);
                freeSpaces.Remove(wallPos);
                // Store the wall in the dictionary
                //if (!wallDict.ContainsKey(wallPos)) // In case the same position is attempted to be used again
                //{
                //    wallDict[wallPos] = wall;
                //}
            }
        }
    }

    void CreateObstacle(Vector2 corner1, Vector2 corner2)
    {
        // Determine the corners of the rectangle
        int left = Mathf.Min((int)corner1.x, (int)corner2.x);
        int right = Mathf.Max((int)corner1.x, (int)corner2.x);
        int bottom = Mathf.Min((int)corner1.y, (int)corner2.y);
        int top = Mathf.Max((int)corner1.y, (int)corner2.y);


        // Create the walls
        for (int i = left; i <= right; i++)
        {
            for (int j = bottom; j <= top; j++)
            {
                Vector3 wallPos = new Vector3(_xSpace * i, 0, _ySpace * j);
                GameObject wall = Instantiate(_wall, wallPos, Quaternion.identity, gridList.transform);

                // Store the wall in the dictionary
                if (!wallDict.ContainsKey(wallPos)) // In case the same position is attempted to be used again
                {
                    wallDict[wallPos] = wall;
                }
            }
        }
    }

    void RemoveObstacle(Vector2 corner1, Vector2 corner2)
    {
        // Determine the corners of the rectangle
        int left = Mathf.Min((int)corner1.x, (int)corner2.x);
        int right = Mathf.Max((int)corner1.x, (int)corner2.x);
        int bottom = Mathf.Min((int)corner1.y, (int)corner2.y);
        int top = Mathf.Max((int)corner1.y, (int)corner2.y);

        // Remove the walls
        for (int i = left; i <= right; i++)
        {
            for (int j = bottom; j <= top; j++)
            {
                Vector3 wallPos = new Vector3(_xSpace * i, 0, _ySpace * j);

                // Check if a wall exists at this position
                if (wallDict.ContainsKey(wallPos))
                {
                    // Destroy the wall and remove it from the dictionary
                    Destroy(wallDict[wallPos]);
                    wallDict.Remove(wallPos);
                }
            }
        }
    }


    void CreateWallSpaceWithExitFreedom(Vector2 corner1, Vector2 corner2, WallSide exitSide)
    {
        // Determine the corners of the rectangle
        int left = Mathf.Min((int)corner1.x, (int)corner2.x);
        int right = Mathf.Max((int)corner1.x, (int)corner2.x);
        int bottom = Mathf.Min((int)corner1.y, (int)corner2.y);
        int top = Mathf.Max((int)corner1.y, (int)corner2.y);

        // Calculate the exit position
        int exitPos;
        if (exitSide == WallSide.Left || exitSide == WallSide.Right)
            exitPos = (bottom + top) / 2;
        else
            exitPos = (left + right) / 2;

        // Create the walls
        for (int i = left; i <= right; i++)
        {
            if (i == left || i == right)
            {
                for (int j = bottom; j <= top; j++)
                {
                    // Skip the exit
                    if ((exitSide == WallSide.Left && i == left && j == exitPos) ||
                        (exitSide == WallSide.Right && i == right && j == exitPos))
                    {
                        continue;
                    }

                    // Instantiate the wall
                    Vector3 wallPos = new Vector3(_xSpace * i, 0, _ySpace * j);
                    Instantiate(_wall, wallPos, Quaternion.identity, obstacleList.transform);
                    freeSpaces.Remove(wallPos);
                }
            }
            else
            {
                // Instantiate bottom wall
                if (!((exitSide == WallSide.Bottom) && (i == exitPos)))
                {
                    Vector3 bottomWallPos = new Vector3(_xSpace * i, 0, _ySpace * bottom);
                    Instantiate(_wall, bottomWallPos, Quaternion.identity, obstacleList.transform);
                    freeSpaces.Remove(bottomWallPos);
                }
                // Instantiate top wall
                if (!((exitSide == WallSide.Top) && (i == exitPos)))
                {
                    Vector3 topWallPos = new Vector3(_xSpace * i, 0, _ySpace * top);
                    Instantiate(_wall, topWallPos, Quaternion.identity, obstacleList.transform);
                    freeSpaces.Remove(topWallPos);
                }
            }
        }
    }

    public GameObject RespawnPacMan()
    {
        freeSpaces.Remove(_birthPlace);
        return Instantiate(_pacManPrefab, _birthPlace, Quaternion.identity);
    }

    public GameObject RespawnBlinky()
    {
        freeSpaces.Remove(_birthPlaceBlinky);
        return Instantiate(_blinkyPrefab, _birthPlaceBlinky, Quaternion.identity);
    }

    public GameObject RespawnClyde()
    {
        freeSpaces.Remove(_birthPlaceClyde);
        return Instantiate(_clydePrefab, _birthPlaceClyde, Quaternion.identity);
    }

    public GameObject RespawnPinky()
    {
        freeSpaces.Remove(_birthPlacePinky);
        return Instantiate(_pinkyPrefab, _birthPlacePinky, Quaternion.identity);
    }

    public GameObject RespawnInky()
    {
        freeSpaces.Remove(_birthPlaceInky);
        return Instantiate(_inkyPrefab, _birthPlaceInky, Quaternion.identity);
    }

    public void SpawnBlinky()
    {
        Instantiate(_blinkyPrefab, _birthPlaceBlinky, Quaternion.identity);
    }

    public void SpawnClyde()
    {
        Instantiate(_clydePrefab, _birthPlaceClyde, Quaternion.identity);
    }

    public void SpawnPinky()
    {
        Instantiate(_pinkyPrefab, _birthPlacePinky, Quaternion.identity);
    }

    public void SpawnInky()
    {
        Instantiate(_inkyPrefab, _birthPlaceInky, Quaternion.identity);
    }

    public void SpawnPowerItem(Vector3 position)
    {
        freeSpaces.Remove(position);
        Instantiate(_powerPrefab, position, Quaternion.identity);
    }

    void SpawnCoins(List<Vector3> freeSpaces)
    {
        foreach (Vector3 freeSpace in freeSpaces)
        {
            // Here we add an offset to the y-axis
            Vector3 coinPosition = new Vector3(freeSpace.x, freeSpace.y + 2f, freeSpace.z);
            Instantiate(_coinPrefab, coinPosition, Quaternion.identity, collectableList.transform);
        }
    }

    //void UpdateNavMesh()
    //{
    //    var navMeshData = new NavMeshData();
    //    NavMesh.AddNavMeshData(navMeshData);
    //    var sources = new List<NavMeshBuildSource>();
    //    foreach (var meshFilter in FindObjectsOfType<MeshFilter>())
    //    {
    //        var meshBuildSource = new NavMeshBuildSource
    //        {
    //            shape = NavMeshBuildSourceShape.Mesh,
    //            sourceObject = meshFilter.sharedMesh,
    //            transform = meshFilter.transform.localToWorldMatrix,
    //            area = 0
    //        };
    //        sources.Add(meshBuildSource);
    //    }
    //    var defaultBuildSettings = NavMesh.GetSettingsByID(0);
    //    NavMeshBuilder.UpdateNavMeshData(navMeshData, defaultBuildSettings, sources, new Bounds(Vector3.zero, new Vector3(500f, 500f, 500f)));
    //}



    // Update is called once per frame
    void Update()
    {
        // For Debug
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameManager.coinsCollected += 100;
            print(freeSpaces.Count);
        }



        //Debug.Log("Current time: " + _timeTracker.GetCurrentTime());

        if (GameManager.coinsCollected > (freeSpaces.Count / 2))
        {
            RemoveObstacle(new Vector2(9, 4), new Vector2(15, 9));
            //UpdateNavMesh();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            // toggle minimap visibility
            minimap.enabled = !minimap.enabled;
        }

        // Check if M key was pressed for toggling minimap
        if (Input.GetKeyDown(KeyCode.M))
        {
            // Toggle active state of minimap
            minimap.gameObject.SetActive(!minimap.gameObject.activeSelf);
        }

        // Resize the minimap based on screen resolution
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float minimapWidth = screenWidth / 5;  // Adjust these values to whatever you prefer
        float minimapHeight = screenHeight / 5;

        // Get the RectTransform of the minimap
        RectTransform rectTransform = minimap.GetComponent<RectTransform>();

        // Update the size of the minimap
        rectTransform.sizeDelta = new Vector2(minimapWidth, minimapHeight);

        if (GameManager.coinsCollected >= freeSpaces.Count)
        {
            SceneManager.LoadScene(WinScreen);
        }
    }

    // Method to get _col * _xSpace
    public float GetMapWidth()
    {
        return _col * _xSpace;
    }

    // Method to get _row * _ySpace
    public float GetMapHeight()
    {
        return _row * _ySpace;
    }
}
