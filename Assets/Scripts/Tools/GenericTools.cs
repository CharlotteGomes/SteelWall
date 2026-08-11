using UnityEngine;

public static class GenericTools
{
    public static bool CheckHorizontal()
    {
        if (InputManager.horizontal != 0) return true;
        return false;
    }

    public static bool CheckVertical()
    {
        if (InputManager.vertical != 0) return true;
        return false;
    }
}
