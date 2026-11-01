using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private PJBattleHUD panel;

    private Image image;
    public SkillSO so { get; private set; }

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
        if (image == null)
            image = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (so == null || panel == null) return;
        panel.SetSkill(so);
    }

    public void UpdateSkillSO(SkillSO skill)
    {
        so = skill;
    }

    public void UpdateSprite()
    {
        if (image == null) CacheReferences();
        if (image == null) return;

        image.sprite = so != null ? so.icon : null;
        image.enabled = so != null && so.icon != null;
    }
}
