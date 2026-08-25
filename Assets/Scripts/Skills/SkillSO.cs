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
    public SkillEffectSO[] effects;
    public AnimationClip clip;
    //public SkillEffectSO[] finisher;
    //public float qte_start = 0;
    //public float qte_end = 0;
}
