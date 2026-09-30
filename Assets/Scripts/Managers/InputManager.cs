using UnityEngine;

public static class InputManager
{
    //Inputs
    public static int horizontal=0;
    public static int vertical =0;
    public static bool esc = false;
    public static int hud_horizontal = 0;
    public static int hud_vertical = 0;
    public static bool interact = false;
    public static bool back = false;
    public static void GetInputs()
    {
        //When we use a boolean followed by a question mark, we are literally creating a yes/no question; immediately after, we define a value for the "yes" case and a value for the "no" case, separated by a :
        horizontal = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0); 
        vertical = (Input.GetKey(KeyCode.S) ? 1 : 0) - (Input.GetKey(KeyCode.W) ? 1 : 0);
        interact = Input.GetKeyDown(KeyCode.E) ? true : false;
        back = Input.GetKeyDown(KeyCode.Q) ? true : false;
        hud_horizontal = (Input.GetKeyDown(KeyCode.D) ? 1 : 0) - (Input.GetKeyDown(KeyCode.A) ? 1 : 0);
        hud_vertical = (Input.GetKeyDown(KeyCode.S) ? 1 : 0) - (Input.GetKeyDown(KeyCode.W) ? 1 : 0);
        esc = Input.GetKeyDown(KeyCode.Escape) ? true : false;
    }
}
