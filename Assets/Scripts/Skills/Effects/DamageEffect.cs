using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Scriptable Objects/SkillSO/Effects/DamageTarget")]
public class DamageTarget: SkillEffectSO
{
    public TypeDmg type;
    public Scaling[] stats;
    public int[] scaling;
    public int damage;

    public override void Execute(BattleContext ctx)
    {
        int dmg = damage;
        for(int i = 0; i < stats.Length; i++)
        {
            dmg += (ctx.Self.Stats(stats[i]) * 100) / scaling[i];
            Debug.Log(dmg);
        }
        ctx.Target.TakeDamage(type, dmg);
    }
}
