using UnityEngine;

[CreateAssetMenu(fileName = "SkillSO", menuName = "Scriptable Objects/SkillSO/Skill")]
public class SkillSO : ScriptableObject
{
    public string Name, Description;
    public Sprite spr;
    public int cooldown;
    public int cost;
    public int precision;
    public SkillEffectSO[] effects;
    public void ExecuteSkill(BattleContext ctx)
    {
        foreach(SkillEffectSO f in effects)
        {
            Debug.Log($"Executou {f}");
            f.Execute(ctx);
        }
    }
}
