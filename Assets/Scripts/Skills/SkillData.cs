using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using UnityEngine;

[System.Serializable]
public class SkillData
{
    public SkillSO original_skill { get; private set; }
    public int acc_base = 100;
    public int crit_mod = 0;

    public List<SkillEffectSO> effects = new List<SkillEffectSO>();

    public BattleContext ctx { get; private set; }
    public List<BattleChar> targets { get; private set; } = new List<BattleChar>();
    
    public SkillData(SkillSO skill, BattleContext ctx)
    {
        this.ctx = ctx;
        original_skill = ScriptableObject.Instantiate(skill);
        SetSkill();
        SetTargets();
    }

    public void SetSkill()
    {
        acc_base = original_skill.acc_base;
        crit_mod = original_skill.crit_mod;
        SetEffects();
    }

    public void SetTargets()
    {
        if (original_skill == null) return;
        targets.Clear();

        if(original_skill.self) { targets.Add(ctx.Self); return; }
        if (original_skill.affect_enemy) { targets = ctx.Enemies.FindAll(x => original_skill.pos.Contains(x.GetSelfPos())).ToList(); return; }
        targets = ctx.Party.FindAll(x => original_skill.pos.Contains(x.GetSelfPos())).ToList();
    }

    public void SetEffects()
    {
        effects.Clear();
        foreach (var effect in original_skill.effects)
        {
            effects.Add(ScriptableObject.Instantiate(effect));
        }
    }

    public void Prepare()
    {
        SetSkill();
        foreach(SkillModifiersSO skillModifiersSO in original_skill.modifiers)
        {
            skillModifiersSO.Prepare(this);
        }
    }

    public void Execute(BattleChar user, BattleResult result)
    {
        foreach(var effect in effects)
        {
            effect.Execute(user, result);
        }
    }
}
