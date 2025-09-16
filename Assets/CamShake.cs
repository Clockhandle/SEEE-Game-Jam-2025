using UnityEngine;
using System.Collections;
public class CamShake : MonoBehaviour
{
    public static CamShake instance;
    public bool start = false;
    public float duration = 1f;
    public AnimationCurve curve;


    private void Awake()
    {
        instance = this;
    }
    void Update()
    {
        if (start)
        {
            start = false;  
            StartCoroutine(ShakingCoroutine());

        }
    }
    
    public void ShakeCam()
    {
        start = true;  // active shking in update
    }

    private IEnumerator ShakingCoroutine()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPosition;
    }
}