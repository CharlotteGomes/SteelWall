using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerState player_state = null; //Current State
    private void Awake()
    {
    }
    void Start()
    {
        player_state = new PlayerWalk(this);   
    }

    void Update()
    {
        if(player_state != null) { player_state.Execute();} //Run the method of current state
    }

    public void ChangeState(PlayerState state)
    {
        player_state = state; 
    }
}
