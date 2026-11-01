using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    public BattleChar character { get; private set; }

    [SerializeField] private LifeBarFill fill;
    [SerializeField] private BackLifeBar backFill;

    private Coroutine backFillRoutine;
    private Canvas parentCanvas;
    private RectTransform rectTransform;
    private Camera canvasCamera;
    private StatusEffectHUD statusHud;
    private StatusEffectTooltipManager sharedTooltipManager;

    private void Awake()
    {
        rectTransform = transform as RectTransform;
        statusHud = GetComponent<StatusEffectHUD>();
        if (statusHud == null)
            statusHud = gameObject.AddComponent<StatusEffectHUD>();
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null && parentCanvas.rootCanvas != null)
            parentCanvas = parentCanvas.rootCanvas;

        if (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            canvasCamera = parentCanvas.worldCamera;
    }

    public void SetCharacter(BattleChar target)
    {
        SetCharacter(target, sharedTooltipManager);
    }

    public void SetCharacter(BattleChar target, StatusEffectTooltipManager sharedTooltipManager)
    {
        character = target;
        this.sharedTooltipManager = sharedTooltipManager;
        if (statusHud != null) statusHud.Initialize(character, this.sharedTooltipManager);
        UpdateBars();
        SetPos();
        SetSize();
    }

    public void UpdateBars()
    {
        if (character == null) return;

        if (fill != null)
            fill.UpdateBar(character);

        if (backFillRoutine != null)
            StopCoroutine(backFillRoutine);

        if (backFill != null)
            backFillRoutine = StartCoroutine(backFill.UpdateBar(character));
    }

    public void SetPos()
    {
        if (character == null) return;
        if (parentCanvas == null)
        {
            parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas != null && parentCanvas.rootCanvas != null)
                parentCanvas = parentCanvas.rootCanvas;
            if (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                canvasCamera = parentCanvas.worldCamera;
        }

        RectTransform parentRect = transform.parent as RectTransform;
        if (parentCanvas == null || rectTransform == null || parentRect == null)
        {
            transform.position = character.transform.position + new Vector3(0f, -0.25f, 0f);
            return;
        }

        Camera eventCamera = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : (canvasCamera != null ? canvasCamera : parentCanvas.worldCamera);

        Vector3 worldPosition = character.transform.position + new Vector3(0f, -0.25f, 0f);
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(eventCamera, worldPosition);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, screenPosition, eventCamera, out Vector2 localPosition))
        {
            rectTransform.anchoredPosition = localPosition;
        }
    }

    public void SetSize()
    {
        transform.localScale = Vector3.one;
    }

    public void SetVisible(bool visible)
    {
        if (statusHud != null) statusHud.SetVisible(visible);
        Graphic[] graphics = GetComponentsInChildren<Graphic>(true);
        foreach (Graphic graphic in graphics)
            if (graphic != null) graphic.enabled = visible;
    }

    private void OnDisable()
    {
        if (backFillRoutine == null) return;
        StopCoroutine(backFillRoutine);
        backFillRoutine = null;
    }
}
