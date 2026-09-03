using UnityEngine;

public class PresentationContext
{
    public BattleChar attacker;
    public BattleChar[] target;

    public Camera camera;

    public CharacterPresentationState attackerState;
    public CharacterPresentationState[] targetState;

    public CameraPresentationState cameraState;

    public PresentationContext(BattleChar attacker, BattleChar[] targets, Camera cam)
    {
        this.attacker = attacker;
        this.target = targets;
        this.camera = cam;

        attackerState = new CharacterPresentationState(attacker.transform.position, attacker.transform.rotation, attacker.sprite_renderer.sprite);
        target = targets;
        for(int i = 0; i < targets.Length; i++)
        {
            BattleChar b = targets[i];
            targetState[i] = new CharacterPresentationState(b.transform.position, b.transform.rotation, b.sprite_renderer.sprite);
        }

        cameraState = new CameraPresentationState(cam.transform.position, cam.orthographicSize);
    }
}
