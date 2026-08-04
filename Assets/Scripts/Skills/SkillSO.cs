using UnityEngine;

[CreateAssetMenu(fileName = "SkillSO", menuName = "Scriptable Objects/SkillSO")]
public class SkillSO : ScriptableObject
{
    public string Name, Description;
    public Sprite spr;
    public SkillEffectSO[] effects;

    public void ExecuteSkill(BattleContext ctx)
    {
        foreach(SkillEffectSO f in effects)
        {
            f.Execute(ctx);
        }
    }
}
