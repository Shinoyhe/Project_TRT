using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{
    // Parameters =================================================================================
    [SerializeField] private Image image;
    [SerializeField, Range(0, 1)] private float startingOpacity;
    [SerializeField] private float fadeDuration = 1f;

    // Public Functions ===========================================================================

    public Coroutine StartFadeIn(float duration = -1f)
    {
        StopAllCoroutines();
        return StartCoroutine(FadeIn(duration > 0 ? duration : fadeDuration));
    }

    public Coroutine StartFadeOut(float duration = -1f)
    {
        StopAllCoroutines();
        return StartCoroutine(FadeOut(duration > 0 ? duration : fadeDuration));
    }

    // Private Methods ============================================================================

    // Start is called before the first frame update
    void Start()
    {
        SetOpacity(startingOpacity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetOpacity(float alpha)
    {
        Color color = image.color;
        color.a = Mathf.Clamp01(alpha);
        image.color = color;
    }

    private IEnumerator FadeIn(float duration)
    {
        float startAlpha = image.color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            SetOpacity(Mathf.Lerp(startAlpha, 1f, time / duration));
            yield return null;
        }

        SetOpacity(1f);
    }

    private IEnumerator FadeOut(float duration)
    {
        float startAlpha = image.color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            SetOpacity(Mathf.Lerp(startAlpha, 0f, time / duration));
            yield return null;
        }

        SetOpacity(0f);
    }
}
