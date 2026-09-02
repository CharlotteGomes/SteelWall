using NUnit.Framework.Constraints;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Scriptable Objects/SkillSO/Effects/Damage")]
public class DamageTarget: SkillEffectSO
{
    public DamageType type;
    public int max_damage;
    public int min_damage;
    public bool unique_value;
    public override void Execute(BattleContext ctx)
    {
        foreach (BattleChar target in ctx.Target)
        {
            int dmg = unique_value ? max_damage : Random.Range(min_damage, max_damage + 1);
            DamageInfo info = new DamageInfo(ctx.Self, target, dmg, type);
            ctx.Self.DealDamage(info);
        }
    }
}
