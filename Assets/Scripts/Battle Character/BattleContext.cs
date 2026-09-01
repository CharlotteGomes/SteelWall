using UnityEngine;
using System.Collections.Generic;

public enum TypeTarget
{
    Self,
    Enemy,
    Party,
    ChooseEnemy,
    ChooseParty
}

[System.Serializable]
public class BattleContext
{
    public BattleChar Self, Target;
    public List<BattleChar> Party;
    public List<BattleChar> Enemies;
    
    public BattleChar RandomChar(List<BattleChar> list, int[] pos)
    {
        if (list.Count == 0) return null;
        List<BattleChar> temp = new();
        foreach (int i in pos)
        {
            if (i < list.Count)
                temp.Add(list[i]);
        }
        if (temp.Count == 0) return null;
        int index = Random.Range(0, temp.Count);
        return temp[index];
    }

    public BattleChar PickChar(List<BattleChar> list, int index)
    {
        if (index < 0 || index >= list.Count) return null;
        return list[index];
    }

    public List<BattleChar> GetListChar(TypeTarget target)
    {
        switch (target)
        {
            case TypeTarget.Enemy:
                return Enemies;
            case TypeTarget.Party:
                return Party;
            case TypeTarget.ChooseEnemy:
                return Enemies;
            case TypeTarget.ChooseParty:
                return Party;
            default:
                return null;
        }
    }
}
