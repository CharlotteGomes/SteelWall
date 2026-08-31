using NUnit.Framework.Constraints;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Scriptable Objects/SkillSO/Effects/DamageTarget")]
public class DamageTarget: SkillEffectSO
{
    public TypeDmg type;
    public ScalingSO[] scaling;
    public int damage;
    public override void Execute(BattleContext ctx, int[] pos, TypeTarget t)
    {
        int dmg = damage;
        for(int i = 0; i < scaling.Length; i++)
        {
            dmg += (ctx.Self.ReadStats(scaling[i].stat) * scaling[i].value) / 100;
            //Debug.Log(dmg);
        }
        switch (t)
        {
            case TypeTarget.Self:
                ctx.Self.TakeDamage(type, dmg); break;

            case TypeTarget.Enemy:
                if(pos == null) { Debug.LogWarning("As posiçõe de efeito não condizem com o tipo da skill"); break; }
                foreach(int i in pos)
                {
                    if(ctx.Enemies[i] != null)
                    {
                        ctx.Enemies[i].TakeDamage(type, dmg);
                    }
                }
                break;

            case TypeTarget.Party:
                if (pos == null) { Debug.LogWarning("As posiçõe de efeito não condizem com o tipo da skill"); break; }
                foreach (int i in pos)
                {
                    if (ctx.Party[i] != null)
                    {
                        ctx.Party[i].TakeDamage(type, dmg);
                    }
                }
                break;

            case TypeTarget.ChooseEnemy:
                ctx.Target.TakeDamage(type, dmg); break;

            case TypeTarget.ChooseParty:
                ctx.Target.TakeDamage(type, dmg);break;
        }
    }
}
