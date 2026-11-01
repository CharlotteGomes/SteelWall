using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Builds and refreshes the status-effect icons for a single character.</summary>
public class StatusEffectHUD : MonoBehaviour
{
    private const int VisibleColumns = 6;
    private const int VisibleRows = 4;
    private const float IconSize = 28f;
    private const float IconSpacing = 3f;

    private BattleChar character;
    private StatusEffectTooltipManager tooltipManager;
    private RectTransform scrollRoot;
    private RectTransform viewport;
    private RectTransform content;
    private ScrollRect scrollRect;
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
        if (scrollRoot != null) return;

        GameObject scrollObject = new GameObject("StatusEffectsScroll", typeof(RectTransform), typeof(ScrollRect));
        scrollObject.transform.SetParent(transform, false);
        scrollRoot = scrollObject.GetComponent<RectTransform>();
        scrollRoot.anchorMin = new Vector2(0.5f, 0.5f);
        scrollRoot.anchorMax = new Vector2(0.5f, 0.5f);
        scrollRoot.pivot = new Vector2(0.5f, 0f);
        scrollRoot.anchoredPosition = new Vector2(0f, 13f);
        scrollRoot.sizeDelta = new Vector2(
            VisibleColumns * IconSize + (VisibleColumns - 1) * IconSpacing + 8f,
            VisibleRows * IconSize + (VisibleRows - 1) * IconSpacing + 8f);

        GameObject viewportObject = new GameObject(
            "Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
        viewportObject.transform.SetParent(scrollRoot, false);
        viewport = viewportObject.GetComponent<RectTransform>();
        viewport.anchorMin = Vector2.zero;
        viewport.anchorMax = Vector2.one;
        viewport.offsetMin = Vector2.zero;
        viewport.offsetMax = Vector2.zero;

        Image viewportImage = viewportObject.GetComponent<Image>();
        viewportImage.color = new Color(0f, 0f, 0f, 0f);
        viewportImage.raycastTarget = true;
        viewportObject.GetComponent<Mask>().showMaskGraphic = false;

        scrollRect = scrollObject.GetComponent<ScrollRect>();
        scrollRect.viewport = viewport;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.inertia = true;
        scrollRect.scrollSensitivity = 24f;

        GameObject contentObject = new GameObject("Content", typeof(RectTransform));
        contentObject.transform.SetParent(viewport, false);
        content = contentObject.GetComponent<RectTransform>();
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);
        content.anchoredPosition = Vector2.zero;
        content.sizeDelta = Vector2.zero;
        scrollRect.content = content;

        GridLayoutGroup grid = contentObject.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(IconSize, IconSize);
        grid.spacing = new Vector2(IconSpacing, IconSpacing);
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment = TextAnchor.UpperLeft;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = VisibleColumns;
        grid.padding = new RectOffset(2, 2, 2, 2);

        ContentSizeFitter fitter = contentObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRoot.gameObject.SetActive(isVisible);
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

        if (statuses != null)
        {
            foreach (StatusEffectData status in statuses)
            {
                if (status == null || status.IsExpired) continue;
                GameObject item = new GameObject("StatusEffect", typeof(RectTransform), typeof(Image));
                item.transform.SetParent(content, false);
                RectTransform itemRect = item.GetComponent<RectTransform>();
                itemRect.sizeDelta = new Vector2(IconSize, IconSize);
                StatusEffectIconView view = item.AddComponent<StatusEffectIconView>();
                view.Initialize(tooltipManager, status);
                icons.Add(view);
            }
        }

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;
    }

    private void HideTooltip()
    {
        if (tooltipManager != null)
            tooltipManager.HideTooltipFrom(this);
    }

    public void SetVisible(bool visible)
    {
        isVisible = visible;
        if (scrollRoot != null) scrollRoot.gameObject.SetActive(visible);
        if (!visible) HideTooltip();
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
