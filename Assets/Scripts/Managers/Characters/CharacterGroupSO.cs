using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Group", menuName = "Scriptable Objects/Party")]
public class CharacterGroupSO : ScriptableObject
{
    public List<PJData> party = new List<PJData>();

    public bool AddParty(PJData c)
    {
        if (c == null) return false;
        party.Add(c);
        return true;
    }

    public bool RemoveParty(PJData c)
    {
        if (!party.Contains(c)) return false;
        party.Remove(c);
        return true;
    }
}
