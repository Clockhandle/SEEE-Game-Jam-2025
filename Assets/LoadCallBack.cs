using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCallBack : MonoBehaviour
{
    //private bool isFirstUpdate = true;

    //private void Update()
    //{
    //    if (isFirstUpdate)
    //    {
    //        isFirstUpdate = false;

    //        LoadSceneManage.LoaderCallback();
    //    }
    //}

    [SerializeField] private float minLoadTime = .5f; // seconds

    private void Start()
    {
        StartCoroutine(WaitAndLoad());
    }

    private IEnumerator WaitAndLoad()
    {
        yield return new WaitForSeconds(minLoadTime);
        LoadSceneManage.LoaderCallback();
    }
}
