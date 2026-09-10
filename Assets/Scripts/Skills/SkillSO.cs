using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillSO", menuName = "Scriptable Objects/SkillSO/Skill")]
public class SkillSO : ScriptableObject
{
    public string Name, Description;
    public Sprite spr;
    public SkillEffectSO[] effects;
    public int[] pos;

    public int acc_base = 100;
    public int crit_mod = 0;
    public bool affect_enemy = true;
    public bool chose = false;
    public bool can_dodge = true;
    public bool can_miss = true;

    public PresentationSO presentation;

    public void Execute(BattleChar user, BattleResult result)
    {
        foreach(var effect in effects)
        {
            effect.Execute(user, result);
        }
    }
}
