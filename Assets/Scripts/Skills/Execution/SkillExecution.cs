/// <summary>
/// Compatibility execution facade around the existing runtime SkillData.
/// Keeps skill orchestration separate from target selection and effect dispatch
/// while preserving the public SkillData.Execute entry point.
/// </summary>
public sealed class SkillExecution
{
    private readonly SkillData skillData;

    public SkillExecution(SkillData skillData)
    {
        this.skillData = skillData;
    }

    public void Execute(BattleChar user, BattleResult result)
    {
        if (skillData == null || skillData.original_skill == null) return;
        EffectExecutor.Execute(user, result, skillData);
    }
}