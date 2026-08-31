using UnityEngine;



[CreateAssetMenu(fileName = "PresentationSO", menuName = "Scriptable Objects/PresentationSO")]
public class PresentationSO : ScriptableObject
{
    public BattleSprites self;
    public BattleSprites targets;
    public Vector2 self_start_pos;
    public Vector2 self_end_pos;
    public Vector2 targets_start_pos;
    public Vector2 targets_end_pos;
}
