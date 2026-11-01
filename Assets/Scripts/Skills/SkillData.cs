using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkillData
{
    // Unity can restore serializable classes without running their constructors.
    // Keep runtime-only selection state rebuildable instead of relying on a field initializer.
    [NonSerialized] private List<BattleChar> selectedTargets;

    public SkillSO original_skill { get; private set; }
    public int acc_base = 100;
    public int crit_mod = 0;
    public List<SkillEffectSO> effects = new List<SkillEffectSO>();
    public BattleContext ctx { get; private set; }
    public List<BattleChar> targets = new List<BattleChar>();
    public IReadOnlyList<BattleChar> selected_targets
    {
        get
        {
            EnsureCollections();
            return selectedTargets.AsReadOnly();
        }
    }

    public SkillData(SkillSO skill, BattleContext context)
    {
        EnsureCollections();
        original_skill = skill;
        ctx = context;
        SetSkill();
        SetTargets();
    }

    private void EnsureCollections()
    {
        if (selectedTargets == null) selectedTargets = new List<BattleChar>();
        if (effects == null) effects = new List<SkillEffectSO>();
        if (targets == null) targets = new List<BattleChar>();
    }

    public void SetSkill()
    {
        EnsureCollections();
        if (original_skill == null) return;
        acc_base = original_skill.acc_base;
        crit_mod = original_skill.crit_mod;
        SetEffects();
    }

    private void SetEffects()
    {
        EnsureCollections();
        ReleaseEffects();
        if (original_skill == null || original_skill.effects == null) return;

        foreach (SkillEffectSO effect in original_skill.effects)
        {
            if (effect != null)
                effects.Add(UnityEngine.Object.Instantiate(effect));
        }
    }

    public void SetTargets()
    {
        EnsureCollections();
        ClearSelectedTargets();
        targets.Clear();
        targets.AddRange(TargetResolver.Resolve(original_skill, ctx));
    }

    /// <summary>
    /// Selects only eligible candidates and mirrors the selection to BattleContext
    /// for legacy modifiers and systems that still consume ctx.Target.
    /// </summary>
    public void SetSelectedTargets(IEnumerable<BattleChar> selection)
    {
        EnsureCollections();
        selectedTargets.Clear();
        if (selection != null)
        {
            foreach (BattleChar candidate in selection)
            {
                if (candidate == null || !candidate.CheckAlive() ||
                    !targets.Contains(candidate) || selectedTargets.Contains(candidate))
                    continue;

                selectedTargets.Add(candidate);
            }
        }

        SyncContextTargets();
    }

    public bool SelectTarget(int index)
    {
        EnsureCollections();
        if (index < 0 || index >= targets.Count) return false;
        SetSelectedTargets(new[] { targets[index] });
        return selectedTargets.Count == 1;
    }

    public void SelectAllTargets()
    {
        EnsureCollections();
        SetSelectedTargets(targets);
    }

    public void ClearSelectedTargets()
    {
        EnsureCollections();
        selectedTargets.Clear();
        SyncContextTargets();
    }

    private void SyncContextTargets()
    {
        EnsureCollections();
        if (ctx == null) return;
        if (ctx.Target == null) ctx.Target = new List<BattleChar>();
        ctx.Target.Clear();
        ctx.Target.AddRange(selectedTargets);
    }

    public void Prepare()
    {
        EnsureCollections();
        if (original_skill == null) return;
        SyncContextTargets();
        SetSkill();
        if (original_skill.modifiers == null) return;
        foreach (SkillModifiersSO modifier in original_skill.modifiers)
            if (modifier != null) modifier.Prepare(this);
    }

    public void Execute(BattleChar user, BattleResult result)
    {
        EnsureCollections();
        new SkillExecution(this).Execute(user, result);
    }

    public void Dispose()
    {
        EnsureCollections();
        ClearSelectedTargets();
        ReleaseEffects();
        targets.Clear();
    }

    private void ReleaseEffects()
    {
        EnsureCollections();
        foreach (SkillEffectSO effect in effects)
            if (effect != null) UnityEngine.Object.Destroy(effect);
        effects.Clear();
    }
}
