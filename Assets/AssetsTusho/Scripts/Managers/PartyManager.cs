using UnityEngine;
using System.Collections.Generic;
public class PartyManager : MonoBehaviour
{
    public List<PJStats> Party;

    public void AddPartyMember(PJStats pj)
    {
        if (Party.Contains(pj)) return;
        Party.Add(pj);
    }

    public void RemovePartyMember(PJStats pj)
    {
        if(!Party.Contains(pj)) return;
        Party.Remove(pj);
    }

    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
