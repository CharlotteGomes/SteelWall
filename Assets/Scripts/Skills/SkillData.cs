using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SkillData
{
    public SkillSO original_skill { get; private set; }
    public int acc_base = 100;
    public int crit_mod = 0;
    public List<SkillEffectSO> effects = new List<SkillEffectSO>();
    public BattleContext ctx { get; private set; }
    public List<BattleChar> targets = new List<BattleChar>();

    public SkillData(SkillSO skill, BattleContext context)
    {
        original_skill = skill;
        ctx = context;
        SetSkill();
        SetTargets();
    }

    public void SetSkill()
    {
        if (original_skill == null) return;
        acc_base = original_skill.acc_base;
        crit_mod = original_skill.crit_mod;
        SetEffects();
    }

    public void SetTargets()
    {
        targets.Clear();
        if (original_skill == null || ctx == null) return;
        if (original_skill.self)
        {
            if (ctx.Self != null && ctx.Self.CheckAlive()) targets.Add(ctx.Self);
            return;
        }

        List<BattleChar> candidates = original_skill.affect_enemy ? ctx.Enemies : ctx.Party;
        if (candidates == null || original_skill.pos == null) return;
        foreach (BattleChar candidate in candidates)
        {
            if (candidate == null || candidate is BattleObjects || !candidate.CheckAlive()) continue;
            if (original_skill.pos.Contains(candidate.GetSelfPos()) && !targets.Contains(candidate))
                targets.Add(candidate);
        }
    }

    public void SetEffects()
    {
        ReleaseEffects();
        if (original_skill == null || original_skill.effects == null) return;
        foreach (SkillEffectSO effect in original_skill.effects)
            if (effect != null) effects.Add(Object.Instantiate(effect));
    }

    public void Prepare()
    {
        if (original_skill == null) return;
        SetSkill();
        if (original_skill.modifiers == null) return;
        foreach (SkillModifiersSO modifier in original_skill.modifiers)
            if (modifier != null) modifier.Prepare(this);
    }

    public void Execute(BattleChar user, BattleResult result)
    {
        foreach (SkillEffectSO effect in effects.ToArray())
            if (effect != null) effect.Execute(user, result);
    }

    public void Dispose()
    {
        ReleaseEffects();
        targets.Clear();
    }

    private void ReleaseEffects()
    {
        foreach (SkillEffectSO effect in effects)
            if (effect != null) Object.Destroy(effect);
        effects.Clear();
    }
}