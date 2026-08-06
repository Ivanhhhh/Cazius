using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashScreen : MonoBehaviour
{

    [SerializeField] private Image _blackFadeImage;

    [SerializeField] private float _fadeInDuration = 1.0f;
    [SerializeField] private float _splashDuration = 2.0f;
    [SerializeField] private float _fadeOutDuration = 1.0f;

    [SerializeField] private SceneField _mainMenuScene;

    private void Start()
    {
        StartCoroutine(PlaySplashScreen());
    }


    private IEnumerator PlaySplashScreen()
    {
        SetAlpha(1f);

        yield return FadeImage(1f, 0f, _fadeInDuration);

        yield return new WaitForSeconds(_splashDuration);

        yield return FadeImage(0f, 1f, _fadeOutDuration);

        SceneManager.LoadScene(_mainMenuScene);
    }

    private IEnumerator FadeImage(float startAlpha, float endAlpha, float duration)
    {
        float eTime = 0f;

        while (eTime < duration)
        {
            eTime += Time.deltaTime;

            float percentage = Mathf.Clamp01(eTime / duration);
            float alpha = Mathf.Lerp(startAlpha, endAlpha, percentage);

            SetAlpha(alpha);

            yield return null;
        }
    }


    private void SetAlpha(float b)
    {
        Color color = _blackFadeImage.color;

        color.a = b;
        _blackFadeImage.color = color;
    }
}
