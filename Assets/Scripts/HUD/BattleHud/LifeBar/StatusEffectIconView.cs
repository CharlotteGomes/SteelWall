using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Pointer target and compact runtime values for one status icon.</summary>
public class StatusEffectIconView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private StatusEffectHUD owner;
    private StatusEffectData status;

    public void Initialize(StatusEffectHUD hud, StatusEffectData data)
    {
        owner = hud;
        status = data;

        Image background = GetComponent<Image>();
        if (background == null)
            background = gameObject.AddComponent<Image>();

        // An Image without a sprite does not generate a UI mesh and may not be
        // considered by GraphicRaycaster, even when raycastTarget is enabled.
        Sprite raycastSprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
        if (raycastSprite != null)
            background.sprite = raycastSprite;
        background.type = Image.Type.Simple;
        background.color = new Color(0.12f, 0.14f, 0.18f, 0.96f);
        background.raycastTarget = true;

        GameObject iconObject = new GameObject("Icon", typeof(RectTransform), typeof(Image));
        iconObject.transform.SetParent(transform, false);
        RectTransform iconRect = iconObject.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.1f, 0.2f);
        iconRect.anchorMax = new Vector2(0.9f, 0.95f);
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;

        Image icon = iconObject.GetComponent<Image>();
        icon.sprite = status.sfSO != null ? status.sfSO.icon : null;
        icon.color = icon.sprite != null ? Color.white : new Color(0.72f, 0.76f, 0.82f, 1f);
        icon.preserveAspect = true;
        icon.raycastTarget = false;

        if (icon.sprite == null)
        {
            GameObject initialsObject = new GameObject("FallbackLabel", typeof(RectTransform), typeof(TextMeshProUGUI));
            initialsObject.transform.SetParent(iconObject.transform, false);
            RectTransform initialsRect = initialsObject.GetComponent<RectTransform>();
            initialsRect.anchorMin = Vector2.zero;
            initialsRect.anchorMax = Vector2.one;
            initialsRect.offsetMin = Vector2.zero;
            initialsRect.offsetMax = Vector2.zero;
            TextMeshProUGUI initials = initialsObject.GetComponent<TextMeshProUGUI>();
            initials.font = TMP_Settings.defaultFontAsset;
            initials.fontSize = 10;
            initials.alignment = TextAlignmentOptions.Center;
            initials.color = new Color(0.08f, 0.09f, 0.12f, 1f);
            string name = status.sfSO != null ? status.sfSO.effectName : status.effectName;
            initials.text = string.IsNullOrEmpty(name) ? "?" : name.Substring(0, 1).ToUpperInvariant();
            initials.raycastTarget = false;
        }

        GameObject valueObject = new GameObject("StacksDuration", typeof(RectTransform), typeof(TextMeshProUGUI));
        valueObject.transform.SetParent(transform, false);
        RectTransform valueRect = valueObject.GetComponent<RectTransform>();
        valueRect.anchorMin = new Vector2(0f, 0f);
        valueRect.anchorMax = new Vector2(1f, 0.3f);
        valueRect.offsetMin = Vector2.zero;
        valueRect.offsetMax = Vector2.zero;

        TextMeshProUGUI values = valueObject.GetComponent<TextMeshProUGUI>();
        values.font = TMP_Settings.defaultFontAsset;
        values.fontSize = 8;
        values.fontStyle = FontStyles.Bold;
        values.alignment = TextAlignmentOptions.Center;
        values.color = Color.white;
        values.text = status.stacks + " | " + (status.remainingDuration < 0 ? "∞" : status.remainingDuration.ToString());
        values.raycastTarget = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (owner != null) owner.ShowTooltip(status, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (owner != null) owner.HideTooltip();
    }
}
