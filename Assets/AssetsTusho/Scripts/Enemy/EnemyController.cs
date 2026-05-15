using UnityEngine;
using System.Collections;
public class EnemyController : BattleCharacter, IDamageable, IKillable
{
    public override IEnumerator ExecuteTurn()
    {
        int skillIdx = Random.Range(0, SkillList.Count);
        SkillBase currentSkill = SkillList[skillIdx];
        int targetIdx;

        if (currentSkill.tTarget == TargetType.Enemies) { targetIdx = Random.Range(0, ctx.enemies.Count); ctx.target = ctx.enemies[targetIdx]; }
        else { targetIdx = Random.Range(0, ctx.allies.Count); ctx.target = ctx.allies[targetIdx]; }

        yield return StartCoroutine(currentSkill.Execute(ctx));
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
