using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Visual and pointer target for one status effect.
/// Uses the assigned status sprite when available and a high-contrast text badge otherwise.
/// </summary>
public class StatusEffectIconView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private StatusEffectTooltipManager tooltipManager;
    private StatusEffectData status;

    public void Initialize(StatusEffectTooltipManager sharedTooltipManager, StatusEffectData data)
    {
        tooltipManager = sharedTooltipManager;
        status = data;

        Image background = GetComponent<Image>();
        if (background == null)
            background = gameObject.AddComponent<Image>();

        // The root Image is deliberately a solid-color badge. It does not depend on
        // Unity's internal UI/Skin/UISprite resource or on an optional project sprite.
        background.sprite = null;
        background.type = Image.Type.Simple;
        background.color = GetFallbackColor(status);
        background.raycastTarget = true;
        background.alphaHitTestMinimumThreshold = 0f;

        Sprite statusSprite = status != null && status.sfSO != null ? status.sfSO.icon : null;
        if (statusSprite != null)
        {
            GameObject iconObject = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            iconObject.transform.SetParent(transform, false);

            RectTransform iconRect = iconObject.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.08f, 0.08f);
            iconRect.anchorMax = new Vector2(0.92f, 0.92f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;

            Image icon = iconObject.GetComponent<Image>();
            icon.sprite = statusSprite;
            icon.color = Color.white;
            icon.preserveAspect = true;
            icon.raycastTarget = false;
        }
        else
        {
            // Most current StatusEffectSO assets have icon == null. Render a clear,
            // readable badge instead of an empty Image when art has not been assigned.
            GameObject fallbackObject = new GameObject(
                "FallbackIcon", typeof(RectTransform), typeof(TextMeshProUGUI));
            fallbackObject.transform.SetParent(transform, false);

            RectTransform fallbackRect = fallbackObject.GetComponent<RectTransform>();
            fallbackRect.anchorMin = new Vector2(0.04f, 0.04f);
            fallbackRect.anchorMax = new Vector2(0.96f, 0.96f);
            fallbackRect.offsetMin = Vector2.zero;
            fallbackRect.offsetMax = Vector2.zero;

            TextMeshProUGUI fallback = fallbackObject.GetComponent<TextMeshProUGUI>();
            fallback.font = ResolveFont();
            fallback.fontSize = 13;
            fallback.fontStyle = FontStyles.Bold;
            fallback.alignment = TextAlignmentOptions.Center;
            fallback.color = Color.white;
            fallback.text = GetAbbreviation(status);
            fallback.raycastTarget = false;
            fallback.enableAutoSizing = true;
            fallback.fontSizeMin = 8;
            fallback.fontSizeMax = 13;
        }

        GameObject valueObject = new GameObject(
            "StacksDuration", typeof(RectTransform), typeof(TextMeshProUGUI));
        valueObject.transform.SetParent(transform, false);

        RectTransform valueRect = valueObject.GetComponent<RectTransform>();
        valueRect.anchorMin = new Vector2(0f, 0f);
        valueRect.anchorMax = new Vector2(1f, 0.3f);
        valueRect.offsetMin = Vector2.zero;
        valueRect.offsetMax = Vector2.zero;

        TextMeshProUGUI values = valueObject.GetComponent<TextMeshProUGUI>();
        values.font = ResolveFont();
        values.fontSize = 8;
        values.fontStyle = FontStyles.Bold;
        values.alignment = TextAlignmentOptions.Center;
        values.color = Color.white;
        values.outlineWidth = 0.18f;
        values.outlineColor = Color.black;
        values.text = BuildCompactValues(status);
        values.raycastTarget = false;
    }

    private static string BuildCompactValues(StatusEffectData data)
    {
        if (data == null) return string.Empty;
        System.Collections.Generic.List<string> parts = new System.Collections.Generic.List<string>();
        if (data.stacks > 1) parts.Add("x" + data.stacks);
        if (data.remainingDuration > 0) parts.Add(data.remainingDuration + "t");
        return string.Join(" ", parts);
    }

    private static string GetAbbreviation(StatusEffectData data)
    {
        string name = data != null && data.sfSO != null
            ? data.sfSO.effectName
            : (data != null ? data.effectName : null);

        if (string.IsNullOrWhiteSpace(name))
            return "?";

        string[] words = name.Trim().Split(' ');
        if (words.Length > 1 && words[0].Length > 0 && words[1].Length > 0)
            return (words[0][0].ToString() + words[1][0]).ToUpperInvariant();

        string normalized = name.Trim();
        return normalized.Length > 1
            ? normalized.Substring(0, 2).ToUpperInvariant()
            : normalized.ToUpperInvariant();
    }

    private static Color GetFallbackColor(StatusEffectData data)
    {
        string name = data != null && data.sfSO != null
            ? data.sfSO.effectName
            : (data != null ? data.effectName : null);
        string key = string.IsNullOrWhiteSpace(name) ? string.Empty : name.ToLowerInvariant();

        if (key.Contains("bleed") || key.Contains("sangramento"))
            return new Color(0.62f, 0.12f, 0.16f, 1f);
        if (key.Contains("poison") || key.Contains("veneno"))
            return new Color(0.16f, 0.48f, 0.25f, 1f);
        if (key.Contains("heal") || key.Contains("restor") || key.Contains("cura"))
            return new Color(0.12f, 0.43f, 0.48f, 1f);
        if (key.Contains("slow") || key.Contains("blind") || key.Contains("mark"))
            return new Color(0.38f, 0.25f, 0.58f, 1f);

        return new Color(0.16f, 0.29f, 0.48f, 1f);
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipManager != null && status != null)
            tooltipManager.ShowTooltip(this, status, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipManager != null)
            tooltipManager.HideTooltip(this);
    }

    private void OnDestroy()
    {
        if (tooltipManager != null)
            tooltipManager.HideTooltip(this);
    }
}
