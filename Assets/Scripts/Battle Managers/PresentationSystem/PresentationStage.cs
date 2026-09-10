using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Playables;
public class PresentationStage
{
    List<PlayableContainer> Playables = new List<PlayableContainer>();
    public IReadOnlyList<PlayableContainer> containers => Playables;

    public PresentationStage Add(Playable p, double time)
    {
        PlayableContainer container = new PlayableContainer(p, time);
        Playables.Add(container);
        return this;
    }
}
