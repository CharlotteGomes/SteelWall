using UnityEngine;

public class BattleContext
{
    public BattleChar Self, Target;
    public BattleChar[] Party;
    public BattleChar[] Enemies;
    public BattleContext(BattleChar self)
    {
        Self = self;
    }
}
