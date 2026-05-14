using UnityEditor;
using UnityEngine;

public interface GameStates
{
    public void Enter();
    public void Exit();
    public void Update();
}

public class Explorer : GameStates
{
    public PlayerController controller;
    public Explorer(PlayerController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        controller.enabled = true;
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        controller.enabled = false;
    }
}

public class Combat : GameStates
{
    public Combat()
    {
        
    }
    public void Enter()
    {
    }

    public void Update()
    {

    }

    public void Exit()
    {
    }
}