using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "EnemyBehaviourSO", menuName = "Scriptable Objects/Enemy/BehaviourSO")]
public class EnemyBehaviourSO : ScriptableObject
{
    public List<SkillSO> behaviour = new List<SkillSO>();
    public List<ConditionalSkillSO> conditional = new List<ConditionalSkillSO>();
}
