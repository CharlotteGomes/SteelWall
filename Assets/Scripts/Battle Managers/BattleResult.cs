using System.Collections.Generic;

[System.Serializable]
public sealed class TargetResolution
{
    public BattleChar target { get; private set; }
    public bool dodged { get; private set; }
    public bool critical { get; private set; }
    public int hitRoll { get; private set; }
    public int hitChance { get; private set; }
    public int criticalRoll { get; private set; }

    public TargetResolution(BattleChar target, bool dodged, bool critical, int hitRoll, int hitChance, int criticalRoll)
    {
        this.target = target;
        this.dodged = dodged;
        this.critical = critical;
        this.hitRoll = hitRoll;
        this.hitChance = hitChance;
        this.criticalRoll = criticalRoll;
    }
}

public class BattleResult
{
    // Compatibility collections retained while effects migrate to per-target results.
    public List<BattleChar> dodge = new List<BattleChar>();
    public List<BattleChar> hit = new List<BattleChar>();
    public List<TargetResolution> target_results = new List<TargetResolution>();
    public int acc_base;
    public int acc_roll;
    public bool skill_fail = true;
    public bool crit = false;

    public void AddTargetResult(TargetResolution resolution)
    {
        if (resolution == null || resolution.target == null) return;
        target_results.Add(resolution);
        if (resolution.dodged) dodge.Add(resolution.target);
        else hit.Add(resolution.target);
        crit |= resolution.critical;
    }

    public TargetResolution GetTargetResult(BattleChar target)
    {
        return target_results.Find(result => result != null && result.target == target);
    }
}
