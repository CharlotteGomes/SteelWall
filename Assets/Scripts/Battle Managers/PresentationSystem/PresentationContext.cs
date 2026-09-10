using UnityEngine;

public class PresentationContext
{
    public Camera camera;

    public CharacterPresentationState userState;
    public CharacterPresentationState[] targetState;

    public CameraPresentationState cameraState;

    public PresentationSO presentationSO;

    public PresentationContext(BattleChar user, BattleChar[] targets, Camera cam, PresentationSO so)
    {
        targetState = new CharacterPresentationState[targets.Length];
        this.camera = cam;

        userState = new CharacterPresentationState(user.transform.position, user.transform.rotation, user.sprite_renderer.sprite, user.transform.localScale ,user, user.sprite_renderer);
        for(int i = 0; i < targets.Length; i++)
        {
            BattleChar b = targets[i];
            targetState[i] = new CharacterPresentationState(b.transform.position, b.transform.rotation, b.sprite_renderer.sprite, targets[i].transform.localScale, b, b.sprite_renderer);
        }

        cameraState = new CameraPresentationState(cam.transform.position, cam.orthographicSize);
        presentationSO = so;
    }
}
