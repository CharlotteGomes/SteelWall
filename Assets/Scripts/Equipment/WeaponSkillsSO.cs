using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WeaponSkillsSO", menuName = "Scriptable Objects/Equipment/Weapon/WeaponSkills")]
public class WeaponSkillsSO : ScriptableObject
{
    public List<SkillSO> weapon_skills = new List<SkillSO>();
}
