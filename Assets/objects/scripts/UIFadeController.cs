using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIFadeController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1.0f;

    private CanvasGroup canvasGroup;
    private Coroutine currentFadeCoroutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void FadeIn()
    {
        StartFade(1.0f);
    }

    public void FadeOut()
    {
        StartFade(0.0f);
    }

    private void StartFade(float targetAlpha)
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        currentFadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;

        if (targetAlpha > 0f)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }
        else
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        currentFadeCoroutine = null;
    }
}