using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class CharacterChangeSpriteBehaviour : PlayableBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite spr;
    public int original_sort;
    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
        original_sort = spriteRenderer.sortingOrder;
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        spriteRenderer.sortingOrder += 10;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        spriteRenderer.sprite = spr;
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        
    }
}
