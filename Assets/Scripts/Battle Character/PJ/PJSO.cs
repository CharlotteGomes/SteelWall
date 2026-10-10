using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleCharSO", menuName = "Scriptable Objects/Char/PJ/PJBase")]
public class PJSO : BattleCharSO
{
    public List<SkillSO> skills = new List<SkillSO>();
}
