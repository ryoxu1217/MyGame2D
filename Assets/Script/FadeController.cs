using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeController : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float defaultFadeTime = 1.0f;
    public bool IsFading { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // 初期状態は透明
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;
    }

    public void StartFadeToClearScene(string sceneName, Color fadeColor, float fadeTime = -1f)
    {
        // fadeTimeが指定されていなければdefaultFadeTimeを使う
        if (fadeTime <= 0f)
            fadeTime = defaultFadeTime;

        StartCoroutine(FadeIn(sceneName, fadeColor, fadeTime));
    }

    private IEnumerator FadeIn(string sceneName, Color fadeColor, float fadeTime)
    {
        // フェード始め
        IsFading = true;
        float t = 0f;

        // 色リセット
        fadeColor.a = 0f;
        fadeImage.color = fadeColor;

        while (t < fadeTime)
        {
            t += Time.deltaTime;

            // alpha を 0 → 1 に変化
            fadeColor.a = Mathf.Lerp(0f, 1f, t / fadeTime);
            fadeImage.color = fadeColor;

            yield return null;
        }
        GameManager.Instance.GoToScene(sceneName);

        yield return null;
        
        // 透明
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;
        // フェード終わり
        IsFading = false;

    }
}
