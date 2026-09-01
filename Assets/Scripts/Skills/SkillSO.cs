using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillSO", menuName = "Scriptable Objects/SkillSO/Skill")]
public class SkillSO : ScriptableObject
{
    public string Name, Description;
    public Sprite spr;
    public bool affect_enemy = true;
    public bool chose = false;
    public int[] pos;
    public SkillEffectSO[] effects;
    public PresentationSO presentation;
    
    public void Execute(BattleContext ctx)
    {
        foreach(var effect in effects)
        {
            effect.Execute(ctx);
        }
    }
}
