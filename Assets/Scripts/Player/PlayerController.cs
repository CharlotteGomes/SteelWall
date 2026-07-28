using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerState player_state = null; //Current State

    void Start()
    {
        player_state = new PlayerWalk(this);   
    }

    // Update is called once per frame
    void Update()
    {
        if(player_state != null) { player_state.Execute(); Debug.Log(player_state); }
    }

    public void ChangeState(PlayerState state)
    {
        player_state = state;
    }
}
