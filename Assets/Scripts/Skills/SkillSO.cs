using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillSO", menuName = "Scriptable Objects/SkillSO/Skill")]
public class SkillSO : ScriptableObject
{
    public string Name, Description;
    public Sprite icon;
    public Sprite anim;
    public List<SkillEffectSO> effects;
    public List<SkillModifiersSO> modifiers;
    public int[] pos;
    public int[] use_pos;

    public int acc_base = 100;
    public int crit_mod = 0;
    public bool affect_enemy = true;
    public bool chose = false;
    public bool self=false;
    public bool can_dodge = true;
    public bool can_miss = true;
    public bool can_crit = true;
}
