using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCallBack : MonoBehaviour
{
    // This script is no longer needed since LoadSceneManage now loads scenes directly
    // You can safely delete this script from your LoadingScene2D scene
    
    private void Start()
    {
        Debug.LogWarning("LoadCallBack script is deprecated. LoadSceneManage now loads scenes directly without loading screen.");
    }
}
