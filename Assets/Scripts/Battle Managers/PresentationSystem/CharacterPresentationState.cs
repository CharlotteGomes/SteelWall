using UnityEngine;

public class CharacterPresentationState
{
    public BattleChar battleChar;
    public Transform tf;
    public Vector3 localPosition;
    public Quaternion localRotation;
    public Vector2 localScale;
    public SpriteRenderer spr_render;
    public Sprite sprite;
    public int orderInLayer;
    
    
    
    public CharacterPresentationState(Vector3 pos, Quaternion rotate, Sprite spr, Vector2 localScale, BattleChar b, SpriteRenderer spr_render)
    {
        localPosition = pos;
        localRotation = rotate;
        sprite = spr;
        battleChar = b;
        this.localScale = localScale;
        tf = battleChar.tf;
        this.spr_render = spr_render;
        orderInLayer = spr_render.sortingOrder;
    }
}
