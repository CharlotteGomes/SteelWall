using UnityEngine;

public class CameraPresentationState
{
    public Vector3 localpos;
    public float ortographicSize;

    public CameraPresentationState(Vector3 pos, float size)
    {
        localpos = pos;
        ortographicSize = size;
    }
}
