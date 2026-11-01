using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Displays the runtime status effects owned by the character above this life bar.</summary>
public class StatusEffectHUD : MonoBehaviour
{
    private BattleChar character;
    private RectTransform row;
    private RectTransform tooltip;
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
        if (row != null) return;

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

        tooltip = CreateRect("StatusTooltip", transform as RectTransform);
        tooltip.anchorMin = new Vector2(0.5f, 0.5f);
        tooltip.anchorMax = new Vector2(0.5f, 0.5f);
        tooltip.pivot = new Vector2(0.5f, 0f);
        tooltip.anchoredPosition = new Vector2(0f, 20f);
        tooltip.sizeDelta = new Vector2(220f, 92f);

        Image background = tooltip.gameObject.AddComponent<Image>();
        background.color = new Color(0.055f, 0.065f, 0.085f, 0.97f);
        background.raycastTarget = false;

        tooltipTitle = CreateText("Title", tooltip, 14, FontStyles.Bold);
        tooltipTitle.rectTransform.anchorMin = new Vector2(0f, 1f);
        tooltipTitle.rectTransform.anchorMax = new Vector2(1f, 1f);
        tooltipTitle.rectTransform.pivot = new Vector2(0.5f, 1f);
        tooltipTitle.rectTransform.anchoredPosition = new Vector2(0f, -5f);
        tooltipTitle.rectTransform.sizeDelta = new Vector2(-12f, 20f);
        tooltipTitle.alignment = TextAlignmentOptions.Center;

        tooltipBody = CreateText("Details", tooltip, 11, FontStyles.Normal);
        tooltipBody.rectTransform.anchorMin = new Vector2(0f, 0f);
        tooltipBody.rectTransform.anchorMax = new Vector2(1f, 1f);
        tooltipBody.rectTransform.offsetMin = new Vector2(7f, 6f);
        tooltipBody.rectTransform.offsetMax = new Vector2(-7f, -27f);
        tooltipBody.alignment = TextAlignmentOptions.TopLeft;
        tooltipBody.enableWordWrapping = true;

        tooltip.gameObject.SetActive(false);
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

    public void ShowTooltip(StatusEffectData status)
    {
        if (status == null || tooltip == null) return;

        string title = status.sfSO != null && !string.IsNullOrWhiteSpace(status.sfSO.effectName)
            ? status.sfSO.effectName : status.effectName;
        string description = status.sfSO != null ? status.sfSO.description : null;
        if (string.IsNullOrWhiteSpace(description))
            description = "Efeito ativo aplicado durante o combate.";

        string duration = status.remainingDuration < 0 ? "Indefinida" : status.remainingDuration.ToString();
        tooltipTitle.text = string.IsNullOrWhiteSpace(title) ? "Efeito de status" : title;
        tooltipBody.text = description + "\n" +
            "Acúmulos: " + status.stacks + "   Potência: " + status.potency +
            "\nDuração restante: " + duration;
        tooltip.gameObject.SetActive(true);
        tooltip.SetAsLastSibling();
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
