using UnityEngine;
using System.Collections;
public class CamShake : MonoBehaviour
{
    public static CamShake instance;
    public bool start = false;
    public float duration = 1f;
    public AnimationCurve deathCurve;
    public AnimationCurve explosionCurve;

    private void Awake()
    {
        instance = this;
    }
    void Update()
    {
        //if (start)
        //{
        //    start = false;  
        //    StartCoroutine(ShakingCoroutine());

        //}
    }
    
    private void ShakeCam(float duration, AnimationCurve curve)
    {
        StartCoroutine(ShakingCoroutine(duration, curve));
    }

    private IEnumerator ShakingCoroutine(float duration, AnimationCurve curve)
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

    public void ShakeExplosion() => ShakeCam(1f, explosionCurve);
    public void ShakeDeath() => ShakeCam(0.5f, deathCurve);
}