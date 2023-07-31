using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGrids : MonoBehaviour
{
    [Header("Spawn Grids Setting")]
    [SerializeField] private int _col = 10;
    [SerializeField] private int _row = 10;
    [SerializeField] private float _xSpace = 2f;
    [SerializeField] private float _ySpace = 2f;
    [SerializeField] private GameObject _grid;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _row; i++)
        {
            for (int j = 0; j < _col; j++)
            {
                Vector3 spawnPosition = new Vector3(_xSpace * j, 0, _ySpace * i);
                Instantiate(_grid, spawnPosition, Quaternion.identity, transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

