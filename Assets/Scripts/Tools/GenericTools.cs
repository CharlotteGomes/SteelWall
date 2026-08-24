using UnityEngine;
using UnityEditor.Playables;
using UnityEditor.Animations;
using UnityEngine.Playables;
using System.Collections;
using UnityEngine.Animations;
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

    public static IEnumerator ForceAnimation(Animator animator, AnimationClip clip)
    {
        PlayableGraph graph = PlayableGraph.Create();

        var output = AnimationPlayableOutput.Create(graph, "Animation", animator);
        var playable_clip = AnimationClipPlayable.Create(graph, clip);
        output.SetSourcePlayable(playable_clip);
        graph.Play();
        yield return new WaitForSeconds(clip.length);
        graph.Destroy();

        //animator.Play("battle_idle");
    }
}
