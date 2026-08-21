using UnityEngine;
using System.Collections.Generic;
using System;
public class PartyManager : MonoBehaviour
{
    [SerializeField] List<PjController> party_list = new List<PjController>();
    public List<PjController> party_list_read => party_list;
    public static Action<PjController> party_add;
    public static Action<PjController> party_remove;

    private void Awake()
    {
        party_add += PartyAdd;
        party_remove += PartyRemove; 
    }

    void Start()
    {
        
    }

    
    void Update()
    {
    }

    void PartyAdd(PjController pj)
    {
        if(party_list.Contains(pj) || party_list.Count >= 4){ return; }
        party_list.Add(pj);
    }
    void PartyRemove(PjController pj)
    {
        if (!party_list.Contains(pj) || party_list.Count >= 4) { return; }
        party_list.Remove(pj);
    }
}
