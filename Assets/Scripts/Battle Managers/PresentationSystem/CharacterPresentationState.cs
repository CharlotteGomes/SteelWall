using UnityEngine;

public class CharacterPresentationState
{
    public Vector3 localPosition;
    public Quaternion localRotation;
    public Sprite sprite;

    public CharacterPresentationState(Vector3 pos, Quaternion rotate, Sprite spr)
    {
        localPosition = pos;
        localRotation = rotate;
        sprite = spr;
    }
}
