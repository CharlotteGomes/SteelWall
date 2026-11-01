using System.Collections;
using UnityEngine;

public class ButtonSelect : MonoBehaviour
{
    private RectTransform rectTransform;
    private Coroutine transition;

    private void Awake()
    {
        CacheReferences();
    }

    private void OnEnable()
    {
        CacheReferences();
    }

    private void CacheReferences()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
    }

    public void MoveTo(RectTransform target)
    {
        if (target == null) return;
        if (transition != null)
            StopCoroutine(transition);

        transition = StartCoroutine(LerpTransform(target));
    }

    public IEnumerator LerpTransform(RectTransform target)
    {
        CacheReferences();
        if (rectTransform == null || target == null) yield break;

        Canvas.ForceUpdateCanvases();

        Vector3 startPosition = rectTransform.position;
        Vector3 endPosition = target.position;
        Vector2 startSize = rectTransform.sizeDelta;
        Vector2 endSize = target.sizeDelta + new Vector2(10f, 26f);

        const float duration = 0.35f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (rectTransform == null || target == null) yield break;

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);

            rectTransform.position = Vector3.Lerp(startPosition, endPosition, eased);
            rectTransform.sizeDelta = Vector2.Lerp(startSize, endSize, eased);
            yield return null;
        }

        if (rectTransform != null && target != null)
        {
            rectTransform.position = target.position;
            rectTransform.sizeDelta = endSize;
        }

        transition = null;
    }

    private void OnDisable()
    {
        if (transition == null) return;
        StopCoroutine(transition);
        transition = null;
    }
}
