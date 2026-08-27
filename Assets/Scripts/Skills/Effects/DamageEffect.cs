using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Scriptable Objects/SkillSO/Effects/DamageTarget")]
public class DamageTarget: SkillEffectSO
{
    public TypeDmg type;
    public ScalingSO[] scaling;
    public int damage;

    public override void Execute(BattleContext ctx)
    {
        int dmg = damage;
        for(int i = 0; i < scaling.Length; i++)
        {
            dmg += (ctx.Self.ReadStats(scaling[i].stat) * scaling[i].value) / 100;
            //Debug.Log(dmg);
        }
        ctx.Target.TakeDamage(type, dmg);
        Debug.Log($"{ctx.Self} deu dano em {ctx.Target}");
    }
}
