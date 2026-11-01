using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Owns the single status-effect tooltip for a combat HUD.
/// Individual icons only report pointer events and never create tooltip UI.
/// </summary>
public class StatusEffectTooltipManager : MonoBehaviour
{
    private RectTransform tooltip;
    private RectTransform canvasRect;
    private Canvas rootCanvas;
    private TextMeshProUGUI tooltipTitle;
    private TextMeshProUGUI tooltipBody;
    private Image tooltipBackground;
    private Sprite configuredBackground;
    private StatusEffectIconView activeSource;

    public void Initialize(Canvas canvas, Sprite backgroundSprite = null)
    {
        if (backgroundSprite != null)
            configuredBackground = backgroundSprite;

        if (canvas != null)
            rootCanvas = canvas.rootCanvas;

        if (tooltip == null)
        {
            canvasRect = rootCanvas != null
                ? rootCanvas.transform as RectTransform
                : transform as RectTransform;

            RectTransform tooltipParent = canvasRect != null ? canvasRect : transform as RectTransform;
            tooltip = CreateRect("StatusEffectTooltip", tooltipParent);
            tooltip.anchorMin = Vector2.zero;
            tooltip.anchorMax = Vector2.zero;
            tooltip.pivot = Vector2.zero;
            tooltip.sizeDelta = new Vector2(340f, 164f);

            tooltipBackground = tooltip.gameObject.AddComponent<Image>();
            tooltipBackground.raycastTarget = false;

            tooltipTitle = CreateText("Title", tooltip, 18, FontStyles.Bold);
            tooltipTitle.rectTransform.anchorMin = new Vector2(0f, 1f);
            tooltipTitle.rectTransform.anchorMax = new Vector2(1f, 1f);
            tooltipTitle.rectTransform.pivot = new Vector2(0.5f, 1f);
            tooltipTitle.rectTransform.anchoredPosition = new Vector2(0f, -9f);
            tooltipTitle.rectTransform.sizeDelta = new Vector2(-20f, 30f);
            tooltipTitle.alignment = TextAlignmentOptions.Center;

            tooltipBody = CreateText("Details", tooltip, 14, FontStyles.Normal);
            tooltipBody.rectTransform.anchorMin = Vector2.zero;
            tooltipBody.rectTransform.anchorMax = Vector2.one;
            tooltipBody.rectTransform.offsetMin = new Vector2(14f, 12f);
            tooltipBody.rectTransform.offsetMax = new Vector2(-14f, -46f);
            tooltipBody.alignment = TextAlignmentOptions.TopLeft;
            tooltipBody.enableWordWrapping = true;
            tooltipBody.lineSpacing = 2f;

            tooltip.gameObject.SetActive(false);
        }

        SetBackgroundSprite(configuredBackground);
    }

    private void SetBackgroundSprite(Sprite sprite)
    {
        if (tooltipBackground == null) return;

        tooltipBackground.sprite = sprite;
        tooltipBackground.type = sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        tooltipBackground.color = sprite != null
            ? Color.white
            : new Color(0.055f, 0.065f, 0.085f, 0.98f);
    }

    public void ShowTooltip(StatusEffectIconView source, StatusEffectData status, Vector2 pointerScreenPosition)
    {
        if (source == null || status == null) return;
        if (tooltip == null) Initialize(rootCanvas, configuredBackground);

        activeSource = source;
        string title = status.sfSO != null && !string.IsNullOrWhiteSpace(status.sfSO.effectName)
            ? status.sfSO.effectName : status.effectName;
        string description = status.GetRuntimeDescription();
        if (string.IsNullOrWhiteSpace(description) && status.sfSO != null)
            description = status.sfSO.description;
        if (string.IsNullOrWhiteSpace(description))
            description = "Efeito ativo durante o combate.";

        System.Collections.Generic.List<string> details = new System.Collections.Generic.List<string>();
        if (status.stacks > 1)
            details.Add("Acúmulos: " + status.stacks);
        if (status.potency > 0 && !(status is BleedEffect) && !(status is PoisonEffect) && !(status is HealPerTurnEffect))
            details.Add("Potência: " + status.potency);
        if (status.remainingDuration > 0)
            details.Add("Duração restante: " + status.remainingDuration + (status.remainingDuration == 1 ? " turno" : " turnos"));
        if (status.source != null && status.source != status.owner)
            details.Add("Origem: " + status.source.name);

        tooltipTitle.text = string.IsNullOrWhiteSpace(title) ? "Efeito de status" : title;
        tooltipBody.text = details.Count > 0
            ? description + "\n\n" + string.Join("\n", details)
            : description;

        if (canvasRect != null)
        {
            Camera eventCamera = rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? rootCanvas.worldCamera : null;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, pointerScreenPosition, eventCamera, out Vector2 localPoint))
            {
                Vector2 position = localPoint - canvasRect.rect.min + new Vector2(18f, 18f);
                position.x = Mathf.Clamp(position.x, 0f, Mathf.Max(0f, canvasRect.rect.width - tooltip.sizeDelta.x));
                position.y = Mathf.Clamp(position.y, 0f, Mathf.Max(0f, canvasRect.rect.height - tooltip.sizeDelta.y));
                tooltip.anchoredPosition = position;
            }
        }

        tooltip.gameObject.SetActive(true);
        tooltip.SetAsLastSibling();
    }

    public void HideTooltip(StatusEffectIconView source)
    {
        if (source == null || activeSource != source) return;
        HideTooltip();
    }

    public void HideTooltipFrom(StatusEffectHUD owner)
    {
        if (owner == null || activeSource == null) return;
        if (activeSource.GetComponentInParent<StatusEffectHUD>() == owner)
            HideTooltip();
    }

    public void HideTooltip()
    {
        activeSource = null;
        if (tooltip != null) tooltip.gameObject.SetActive(false);
    }

    private static RectTransform CreateRect(string objectName, RectTransform parent)
    {
        GameObject go = new GameObject(objectName, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go.GetComponent<RectTransform>();
    }

    private static TextMeshProUGUI CreateText(string objectName, RectTransform parent, float size, FontStyles style)
    {
        GameObject go = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
        text.font = ResolveFont();
        text.fontSize = size;
        text.fontStyle = style;
        text.color = Color.white;
        text.raycastTarget = false;
        text.enableWordWrapping = true;
        return text;
    }

    private static TMP_FontAsset ResolveFont()
    {
        TMP_FontAsset font = TMP_Settings.defaultFontAsset;
        if (font != null) return font;

        font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (font != null) return font;

        TMP_FontAsset[] loadedFonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
        return loadedFonts != null && loadedFonts.Length > 0 ? loadedFonts[0] : null;
    }

    private void OnDisable()
    {
        HideTooltip();
    }

    private void OnDestroy()
    {
        HideTooltip();
    }
}
