using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglePlatform : MonoBehaviour
{
    [SerializeField] private GameObject[] platforms;
    [SerializeField] private float cycleTime = 2f;

    private int numberOfPlatforms;
    private int toggleTime;

    private void Start()
    {
        numberOfPlatforms = platforms.Length;

        // Ensure toggleTime has a valid value
        toggleTime = (numberOfPlatforms - 1 == 0) ? 1 : numberOfPlatforms - 1;

        StartCoroutine(ManagePlatformsRoutine());
    }

    private IEnumerator ManagePlatformsRoutine()
    {
        for (int i = 0; i < numberOfPlatforms; i++)
        {
            StartCoroutine(ManageSinglePlatform(platforms[i]));
            yield return new WaitForSeconds(cycleTime);
        }
    }

    private IEnumerator ManageSinglePlatform(GameObject platform)
    {
        // TODO: Add logic for enabling/disabling platform
        while (true)
        {
            platform.SetActive(!platform.activeSelf);
            yield return new WaitForSeconds(cycleTime * toggleTime);
        }
    }
}
