using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class CharacterReturn : PlayableBehaviour
{
    public Transform tf;
    public CharacterPresentationState state;
    public SpriteRenderer spr_render;
    public int sortingOrder = 0;
    public Sprite spr;
    public Vector2 velocity = Vector2.zero;
    public Vector2 start_pos;
    public override void OnGraphStart(Playable playable)
    {
        
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
        spr_render.sortingOrder = sortingOrder;
        spr_render.sprite = spr;
    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        start_pos = tf.localPosition;
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        tf.localPosition = state.localPosition;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        float t = (float)(playable.GetTime() / playable.GetDuration());
        tf.localPosition = Vector2.Lerp(start_pos, state.localPosition, t);
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        
    }
}
