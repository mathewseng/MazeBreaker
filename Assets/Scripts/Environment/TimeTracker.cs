using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeTracker : MonoBehaviour
{
    public float elapsedTime = 0;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        elapsedTime = 0;
    }

    public float GetCurrentTime()
    {
        return elapsedTime;
    }
}
