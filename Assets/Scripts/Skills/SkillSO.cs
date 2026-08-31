using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillSO", menuName = "Scriptable Objects/SkillSO/Skill")]
public class SkillSO : ScriptableObject
{
    public string Name, Description;
    public Sprite spr;
    public int cooldown;
    public int cost;
    public TypeTarget target;
    public int[] pos;
    public SkillEffectSO[] effects;
    public SkillEffectSO[] self_effects;
    public PresentationSO presentation;
    //public SkillEffectSO[] finisher;
    //public float qte_start = 0;
    //public float qte_end = 0;
    
    public void Execute(BattleContext ctx)
    {
        foreach (var effect in self_effects) 
        {
            effect.Execute(ctx, new int[] { }, TypeTarget.Self);
        }

        foreach(var effect in effects)
        {
            effect.Execute(ctx, pos, target);
        }
    }
}
