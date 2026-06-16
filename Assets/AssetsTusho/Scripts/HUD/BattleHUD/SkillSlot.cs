using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    public SkillBase currentSkill;
    [SerializeField] Image SkillIcon;
    [SerializeField] Vector3 velocity;
    [SerializeField] Vector3 sizevelocity;
    RectTransform rect;
    float TargetScale = 1;
    public void ReceiveSkill(SkillBase skill)
    {
        currentSkill = skill;
        SkillIcon.sprite = currentSkill.Icon;
    }

    public void CheckCooldown()
    {
        if (currentSkill.Cooldown > 0) { SkillIcon.color = Color.darkGray; }
        if(currentSkill.Cooldown == 0) { SkillIcon.color = Color.white; }   
    }

    public void SetAnchor(Transform Anchor, float size)
    {
        rect.SetParent(Anchor, true);
        TargetScale = size;
    }
    void Awake()
    {
        rect = GetComponent<RectTransform>();   
    }
    void Update()
    {
        rect.localPosition = Vector3.SmoothDamp(rect.localPosition, Vector3.zero, ref velocity, 0.2f);
        rect.localScale = Vector3.SmoothDamp(rect.localScale, new Vector2(TargetScale, TargetScale), ref sizevelocity, 0.1f);
    }
}
