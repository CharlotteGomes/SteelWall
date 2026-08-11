using System.Collections;
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
    public bool criticable = false;
    public float min_crit_time = 0;
    public float max_crit_time = 0;
    public void ExecuteSkill(BattleContext ctx)
    {
        foreach(SkillEffectSO f in effects)
        {
            Debug.Log($"Executou {f}");
            f.Execute(ctx);
        }
    }

    
}
