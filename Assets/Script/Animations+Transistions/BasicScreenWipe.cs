using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScreenWipe : MonoBehaviour
{
    public static ScreenWipe Instance;

    public Image blackScreen;
    public float duration = 1f;

    void Awake()
    {
        Instance = this;
        
        DontDestroyOnLoad(gameObject);

        // Setup the black screen for horizontal wipe
        blackScreen.type = Image.Type.Filled;
        blackScreen.fillMethod = Image.FillMethod.Horizontal;
        blackScreen.fillOrigin = 0; // left side
        blackScreen.fillAmount = 0f;

    }

    public void FadeIn()  // screen goes black left → right
    {
        //  StopAllCoroutines();
        StartCoroutine(Wipe(0f, 1f));
    }

    public void FadeOut() // screen goes clear left → right
    {
        StopAllCoroutines();
        StartCoroutine(Wipe(1f, 0f));
    }

    private IEnumerator Wipe(float from, float to)
    {
        float time = 0f;
        blackScreen.fillAmount = from;
        while (time < duration)
        {
            time += Time.deltaTime;
            blackScreen.fillAmount = Mathf.Lerp(from, to, time / duration);
            yield return null;
        }
        blackScreen.fillAmount = to;
    }

    public static void TransitionToScene(string sceneName)
    {
        if (Instance != null)
        {
            Instance.StartCoroutine(Instance.TransitionSceneCoroutine(sceneName));
            Debug.Log("This instance is called");
        }
        else
        {
            Debug.LogError("ScreenWipe instance not found. Loading scene immediately.");
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator TransitionSceneCoroutine(string sceneName)
    {
        Debug.Log("Trandsisyin is called");
        // Fade to black
        FadeIn();
        yield return new WaitForSeconds(duration);
        // Load the new scene.
        SceneManager.LoadScene(sceneName);
        // Optionally wait one frame before fading out
        yield return null;
        FadeOut();
    }
}
