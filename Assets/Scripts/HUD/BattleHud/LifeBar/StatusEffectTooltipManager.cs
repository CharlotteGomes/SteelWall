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
    private StatusEffectIconView activeSource;

    public void Initialize(Canvas canvas, Sprite backgroundSprite = null)
    {
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
            tooltip.sizeDelta = new Vector2(250f, 108f);

            tooltipBackground = tooltip.gameObject.AddComponent<Image>();
            tooltipBackground.raycastTarget = false;

            tooltipTitle = CreateText("Title", tooltip, 14, FontStyles.Bold);
            tooltipTitle.rectTransform.anchorMin = new Vector2(0f, 1f);
            tooltipTitle.rectTransform.anchorMax = new Vector2(1f, 1f);
            tooltipTitle.rectTransform.pivot = new Vector2(0.5f, 1f);
            tooltipTitle.rectTransform.anchoredPosition = new Vector2(0f, -5f);
            tooltipTitle.rectTransform.sizeDelta = new Vector2(-12f, 22f);
            tooltipTitle.alignment = TextAlignmentOptions.Center;

            tooltipBody = CreateText("Details", tooltip, 11, FontStyles.Normal);
            tooltipBody.rectTransform.anchorMin = Vector2.zero;
            tooltipBody.rectTransform.anchorMax = Vector2.one;
            tooltipBody.rectTransform.offsetMin = new Vector2(8f, 7f);
            tooltipBody.rectTransform.offsetMax = new Vector2(-8f, -30f);
            tooltipBody.alignment = TextAlignmentOptions.TopLeft;
            tooltipBody.enableWordWrapping = true;

            tooltip.gameObject.SetActive(false);
        }

        SetBackgroundSprite(backgroundSprite);
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
        if (tooltip == null) Initialize(rootCanvas);

        activeSource = source;
        string title = status.sfSO != null && !string.IsNullOrWhiteSpace(status.sfSO.effectName)
            ? status.sfSO.effectName : status.effectName;
        string description = status.sfSO != null ? status.sfSO.description : null;
        if (string.IsNullOrWhiteSpace(description))
            description = GetFallbackDescription(status);

        string duration = status.remainingDuration < 0 ? "Indefinida" : status.remainingDuration.ToString();
        tooltipTitle.text = string.IsNullOrWhiteSpace(title) ? "Efeito de status" : title;
        tooltipBody.text = description + "\n" +
            "Acúmulos: " + status.stacks + "   Potência: " + status.potency +
            "\nDuração restante: " + duration;

        if (canvasRect != null)
        {
            Camera eventCamera = rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? rootCanvas.worldCamera : null;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, pointerScreenPosition, eventCamera, out Vector2 localPoint))
            {
                Vector2 position = localPoint - canvasRect.rect.min + new Vector2(16f, 16f);
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

    private static string GetFallbackDescription(StatusEffectData status)
    {
        if (status is BleedEffect)
            return "Causa dano direto no início do turno do personagem.";
        if (status is PoisonEffect)
            return "Causa dano no final do turno do personagem.";
        if (status is HealPerTurnEffect)
            return "Recupera vida no início do turno do personagem.";
        if (status is StatModifierEffect)
            return "Altera um ou mais atributos enquanto estiver ativo.";
        if (status is DamageModifierEffect)
            return "Modifica o dano causado pelo personagem.";
        if (status is Penitence)
            return "Efeito acumulável que aplica modificadores de atributos.";
        if (status is MarkEffect)
            return "Marca de status ativa neste personagem.";
        return "Efeito de status ativo durante o combate.";
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
        text.font = TMP_Settings.defaultFontAsset;
        text.fontSize = size;
        text.fontStyle = style;
        text.color = Color.white;
        text.raycastTarget = false;
        text.enableWordWrapping = true;
        return text;
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
