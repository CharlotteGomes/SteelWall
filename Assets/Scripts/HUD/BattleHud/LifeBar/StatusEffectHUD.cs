using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Displays runtime status effects and pointer-positioned descriptions for one character.</summary>
public class StatusEffectHUD : MonoBehaviour
{
    private BattleChar character;
    private RectTransform row;
    private RectTransform tooltip;
    private RectTransform canvasRect;
    private Canvas rootCanvas;
    private TextMeshProUGUI tooltipTitle;
    private TextMeshProUGUI tooltipBody;
    private readonly List<StatusEffectIconView> icons = new List<StatusEffectIconView>();
    private string lastSignature = string.Empty;
    private bool isVisible = true;

    public void Initialize(BattleChar owner)
    {
        character = owner;
        EnsureVisuals();
        Refresh(true);
    }

    private void Update()
    {
        if (character != null)
            Refresh(false);
    }

    public void SetCharacter(BattleChar owner)
    {
        character = owner;
        Refresh(true);
    }

    private void EnsureVisuals()
    {
        if (row != null && tooltip != null) return;

        Canvas canvas = GetComponentInParent<Canvas>();
        rootCanvas = canvas != null ? canvas.rootCanvas : null;
        canvasRect = rootCanvas != null ? rootCanvas.transform as RectTransform : null;

        if (row == null)
        {
            row = CreateRect("StatusEffects", transform as RectTransform);
            row.anchorMin = new Vector2(0.5f, 0.5f);
            row.anchorMax = new Vector2(0.5f, 0.5f);
            row.pivot = new Vector2(0.5f, 1f);
            row.anchoredPosition = new Vector2(0f, -16f);
            row.sizeDelta = new Vector2(180f, 34f);
            row.gameObject.SetActive(isVisible);

            HorizontalLayoutGroup layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.spacing = 3f;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
        }

        if (tooltip == null)
        {
            RectTransform tooltipParent = canvasRect != null ? canvasRect : transform as RectTransform;
            tooltip = CreateRect("StatusTooltip", tooltipParent);
            tooltip.anchorMin = Vector2.zero;
            tooltip.anchorMax = Vector2.zero;
            tooltip.pivot = Vector2.zero;
            tooltip.sizeDelta = new Vector2(250f, 108f);

            Image background = tooltip.gameObject.AddComponent<Image>();
            background.color = new Color(0.055f, 0.065f, 0.085f, 0.98f);
            background.raycastTarget = false;

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
    }

    private void Refresh(bool force)
    {
        EnsureVisuals();
        IReadOnlyList<StatusEffectData> statuses = character != null ? character.status_effects : null;
        StringBuilder signature = new StringBuilder();

        if (statuses != null)
        {
            foreach (StatusEffectData status in statuses)
            {
                if (status == null || status.IsExpired) continue;
                signature.Append(status.sfSO != null ? status.sfSO.GetInstanceID() : status.effectName)
                    .Append(':').Append(status.stacks)
                    .Append(':').Append(status.potency)
                    .Append(':').Append(status.remainingDuration).Append('|');
            }
        }

        string nextSignature = signature.ToString();
        if (!force && nextSignature == lastSignature) return;
        lastSignature = nextSignature;

        foreach (StatusEffectIconView icon in icons)
            if (icon != null) Destroy(icon.gameObject);
        icons.Clear();
        HideTooltip();

        if (statuses == null) return;
        foreach (StatusEffectData status in statuses)
        {
            if (status == null || status.IsExpired) continue;
            GameObject item = new GameObject("StatusEffect", typeof(RectTransform), typeof(Image));
            item.transform.SetParent(row, false);
            RectTransform itemRect = item.GetComponent<RectTransform>();
            itemRect.sizeDelta = new Vector2(30f, 30f);
            StatusEffectIconView view = item.AddComponent<StatusEffectIconView>();
            view.Initialize(this, status);
            icons.Add(view);
        }
    }

    public void ShowTooltip(StatusEffectData status, Vector2 pointerScreenPosition)
    {
        if (!isVisible || status == null || tooltip == null) return;

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
            Camera eventCamera = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null : rootCanvas.worldCamera;
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

    public void HideTooltip()
    {
        if (tooltip != null) tooltip.gameObject.SetActive(false);
    }

    public void SetVisible(bool visible)
    {
        isVisible = visible;
        if (row != null) row.gameObject.SetActive(visible);
        if (!visible) HideTooltip();
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
}
