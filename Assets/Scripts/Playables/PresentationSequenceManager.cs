using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;
// A behaviour that is attached to a playable
public class PresentationSequenceManager : PlayableBehaviour
{
    public Presentation presentation;
    PresentationStage current_stage;
    public override void OnGraphStart(Playable playable)
    {
        for(int i = 0; i < playable.GetInputCount(); i++)
        {
            playable.GetInput(i).Pause();
        }
        current_stage = presentation.Stages[0];
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
        
    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        
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
