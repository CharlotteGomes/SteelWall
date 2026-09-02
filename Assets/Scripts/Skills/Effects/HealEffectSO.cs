using UnityEngine;

[CreateAssetMenu(fileName = "HealEffect", menuName = "Scriptable Objects/SkillSO/Effects/Heal")]
public class HealEffectSO : SkillEffectSO
{
    public int max_heal;
    public int min_heal;
    public bool unique_value;

    public override void Execute(BattleContext ctx)
    {
        foreach (BattleChar target in ctx.Target)
        {
            int heal_amount = unique_value ? max_heal : Random.Range(min_heal, max_heal + 1);
            HealInfo info = new HealInfo(ctx.Self, heal_amount, target);
            ctx.Self.DealHeal(info);
        }
    }
}
