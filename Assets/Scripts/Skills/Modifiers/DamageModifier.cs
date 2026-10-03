using UnityEngine;

[CreateAssetMenu(fileName = "DamageModifier", menuName = "Scriptable Objects/SkillSO/Modifiers/DamageModifier")]
public class DamageModifier : SkillModifiersSO
{
    public int damage;
    public override void Prepare(SkillData data)
    {
        foreach (var effects in data.effects)
        {
            if (effects is not DamageTarget d) continue;
            d.dmg += damage;
        }
    }
}
