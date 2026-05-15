using UnityEngine;
using System.Collections;
[CreateAssetMenu(menuName = "Scriptable Objects/Skills/BasicAtack")]
public class BasicAtack : SkillBase
{
    public int baseDamage;
    public int prc;
    public override IEnumerator Execute(BattleContext ctx)
    {
        var TotalDamage = baseDamage + ((ctx.user.Str * prc) / 100);
        Debug.Log($"{ctx.user} atacou {ctx.target}, causando {TotalDamage} de dano");
        ctx.target.TakeDamage(TotalDamage);
        yield return new WaitForSeconds(1);
    }
}