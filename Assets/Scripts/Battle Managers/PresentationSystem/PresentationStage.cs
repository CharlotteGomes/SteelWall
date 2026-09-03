using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Playables;
public class PresentationStage
{
    List<PlayableContainer> Playables = new List<PlayableContainer>();
    public IReadOnlyList<PlayableContainer> containers => Playables;
    public double start_time { get; private set; }

    public PresentationStage(double time)
    {
        start_time = time;
    }
    public PresentationStage Add(Playable p, double time)
    {
        PlayableContainer container = new PlayableContainer(p, time);
        Playables.Add(container);
        return this;
    }
}
