using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    private Vector3 offset = new Vector3(0, 2, -10f);
    public float smoothSpeed;
    public float winCamSize;
    private float defaultCamSize;

    private void Start()
    {
        defaultCamSize = Camera.main.orthographicSize;
        WinFlagGoal.instance.OnTriggerWinFlag += WinFlag_OnTriggerWin;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        Vector3 targetPosition = target.position + offset;   
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime); 
    }

    void WinFlag_OnTriggerWin(object sender, System.EventArgs e)
    {
        Camera.main.orthographicSize = winCamSize;
        transform.position = target.position + offset;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset cam size when entering new level
        Camera.main.orthographicSize = defaultCamSize;
    }
}
