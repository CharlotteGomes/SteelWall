using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Builds and refreshes the status-effect icons for a single character.</summary>
public class StatusEffectHUD : MonoBehaviour
{
    private BattleChar character;
    private StatusEffectTooltipManager tooltipManager;
    private RectTransform row;
    private readonly List<StatusEffectIconView> icons = new List<StatusEffectIconView>();
    private string lastSignature = string.Empty;
    private bool isVisible = true;

    public void Initialize(BattleChar owner, StatusEffectTooltipManager sharedTooltipManager)
    {
        character = owner;
        tooltipManager = sharedTooltipManager;
        EnsureVisuals();
        Refresh(true);
    }

    public void Initialize(BattleChar owner)
    {
        Initialize(owner, tooltipManager);
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

        HideTooltip();
        foreach (StatusEffectIconView icon in icons)
            if (icon != null) Destroy(icon.gameObject);
        icons.Clear();

        if (statuses == null) return;
        foreach (StatusEffectData status in statuses)
        {
            if (status == null || status.IsExpired) continue;
            GameObject item = new GameObject("StatusEffect", typeof(RectTransform), typeof(Image));
            item.transform.SetParent(row, false);
            RectTransform itemRect = item.GetComponent<RectTransform>();
            itemRect.sizeDelta = new Vector2(30f, 30f);
            StatusEffectIconView view = item.AddComponent<StatusEffectIconView>();
            view.Initialize(tooltipManager, status);
            icons.Add(view);
        }
    }

    private void HideTooltip()
    {
        if (tooltipManager != null)
            tooltipManager.HideTooltipFrom(this);
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

    private void OnDisable()
    {
        HideTooltip();
    }

    private void OnDestroy()
    {
        HideTooltip();
    }
}
