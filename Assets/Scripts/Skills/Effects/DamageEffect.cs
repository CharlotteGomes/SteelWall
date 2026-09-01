using NUnit.Framework.Constraints;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Scriptable Objects/SkillSO/Effects/DamageTarget")]
public class DamageTarget: SkillEffectSO
{
    public DamageType type;
    public int damage;
    public override void Execute(BattleContext ctx, int[] pos)
    {
        foreach (var target in ctx.Target)
        {
            DamageInfo info = new DamageInfo(ctx.Self, target, damage, type);
            ctx.Self.DealDamage(info);
        }
    }
}
