using System.Collections;
using UnityEngine;
using TwentyFiveSlicer.Runtime;

public class Indicator : MonoBehaviour
{
    public TwentyFiveSliceImage ind_image, smk_image;
    public bool animate = true;

    [SerializeField] private CanvasGroup group;

    public BattleChar character { get; private set; }

    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private Camera canvasCamera;

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
        if (group == null)
            group = GetComponent<CanvasGroup>();

        if (rectTransform == null)
            rectTransform = transform as RectTransform;

        if (parentCanvas == null)
        {
            parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas != null && parentCanvas.rootCanvas != null)
                parentCanvas = parentCanvas.rootCanvas;

            if (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                canvasCamera = parentCanvas.worldCamera;
        }
    }

    public void SetChar(BattleChar target)
    {
        character = target;
        SetPosition();
    }

    public void SetPosition()
    {
        if (character == null) return;

        RectTransform parentRect = transform.parent as RectTransform;
        if (parentCanvas == null || rectTransform == null || parentRect == null)
        {
            transform.position = (Vector2)character.transform.position + Vector2.down * 0.05f;
            transform.localScale = Vector3.one;
            return;
        }

        Camera eventCamera = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : (canvasCamera != null ? canvasCamera : parentCanvas.worldCamera);

        Vector3 worldPosition = character.transform.position + Vector3.down * 0.05f;
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(eventCamera, worldPosition);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, screenPosition, eventCamera, out Vector2 localPosition))
        {
            rectTransform.anchoredPosition = localPosition;
        }

        transform.localScale = Vector3.one;
    }

    public void SetSprites(Sprite indicator, Sprite smoke)
    {
        if (ind_image != null)
        {
            ind_image.enabled = indicator != null;
            ind_image.sprite = indicator;
        }

        if (smk_image != null)
        {
            smk_image.enabled = smoke != null;
            smk_image.sprite = smoke;
        }
    }

    public void Select(bool selected)
    {
        if (group != null)
            group.alpha = selected ? 1f : 0.3f;
    }

    public IEnumerator StartIndAnimation(BattleChar target)
    {
        if (target == null) yield break;

        float size = transform.localScale.x;
        float originalScale = size;
        float targetScale = 1.25f * size;
        float time = 0f;
        float duration = 0.75f * size;

        while (animate && character != null)
        {
            time += Time.deltaTime / Mathf.Max(duration, 0.01f);
            float curve = 1f - Mathf.Pow(1f - time, 3f);

            transform.localScale = new Vector3(
                Mathf.Lerp(originalScale, targetScale, curve),
                1f,
                transform.localScale.z);

            if (curve >= 0.95f)
            {
                time = 0f;
                duration = Mathf.Approximately(duration, 0.75f * size)
                    ? 0.33f * size
                    : 0.75f * size;

                float previousScale = originalScale;
                originalScale = targetScale;
                targetScale = previousScale;
            }

            yield return null;
        }
    }

    private void OnDestroy()
    {
        animate = false;
    }
}
