using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>Optional Animator and camera reactions. Combat remains functional without this component.</summary>
public sealed class BattlePresentationController : MonoBehaviour
{
    [Header("Animator triggers (only applied when present on the Animator)")]
    [SerializeField] private string attackTrigger = "Attack";
    [SerializeField] private string hitTrigger = "Hit";
    [SerializeField] private string dodgeTrigger = "Dodge";
    [SerializeField] private string completeTrigger = "ActionEnd";

    [Header("Camera reaction")]
    [SerializeField] private Camera battleCamera;
    [SerializeField, Min(0f)] private float impulse = 0.08f;
    [SerializeField, Min(0.01f)] private float impulseDuration = 0.12f;
    [SerializeField, Min(1f)] private float criticalMultiplier = 1.5f;
    private Coroutine cameraRoutine;
    private Vector3 cameraRestPosition;

    private void OnEnable()
    {
        BattleCombatEvents.SkillStarted += HandleSkillStarted;
        BattleCombatEvents.TargetResolved += HandleTargetResolved;
        BattleCombatEvents.SkillCompleted += HandleSkillCompleted;
        if (battleCamera == null) battleCamera = Camera.main;
        if (battleCamera != null) cameraRestPosition = battleCamera.transform.localPosition;
    }

    private void OnDisable()
    {
        BattleCombatEvents.SkillStarted -= HandleSkillStarted;
        BattleCombatEvents.TargetResolved -= HandleTargetResolved;
        BattleCombatEvents.SkillCompleted -= HandleSkillCompleted;
        if (cameraRoutine != null) StopCoroutine(cameraRoutine);
        RestoreCamera();
    }

    private void HandleSkillStarted(BattleChar actor, SkillSO skill) => Trigger(actor, attackTrigger);

    private void HandleTargetResolved(BattleChar actor, SkillSO skill, TargetResolution result)
    {
        if (result == null || result.target == null) return;
        Trigger(result.target, result.dodged ? dodgeTrigger : hitTrigger);
        if (!result.dodged && impulse > 0f && battleCamera != null)
            ShakeCamera(impulse * (result.critical ? criticalMultiplier : 1f));
    }

    private void HandleSkillCompleted(BattleChar actor, SkillSO skill, BattleResult result) => Trigger(actor, completeTrigger);

    private static void Trigger(BattleChar character, string triggerName)
    {
        if (character == null || string.IsNullOrWhiteSpace(triggerName)) return;
        Animator animator = character.GetComponentInChildren<Animator>();
        if (animator == null) return;
        bool exists = animator.parameters.Any(parameter => parameter.type == AnimatorControllerParameterType.Trigger && parameter.name == triggerName);
        if (exists) animator.SetTrigger(triggerName);
    }

    private void ShakeCamera(float strength)
    {
        if (battleCamera == null) return;
        if (cameraRoutine != null) StopCoroutine(cameraRoutine);
        cameraRoutine = StartCoroutine(CameraImpulse(strength));
    }

    private IEnumerator CameraImpulse(float strength)
    {
        float elapsed = 0f;
        while (elapsed < impulseDuration && battleCamera != null)
        {
            elapsed += Time.unscaledDeltaTime;
            float fade = 1f - Mathf.Clamp01(elapsed / impulseDuration);
            Vector2 offset = Random.insideUnitCircle * (strength * fade);
            battleCamera.transform.localPosition = cameraRestPosition + new Vector3(offset.x, offset.y, 0f);
            yield return null;
        }
        RestoreCamera();
        cameraRoutine = null;
    }

    private void RestoreCamera()
    {
        if (battleCamera != null) battleCamera.transform.localPosition = cameraRestPosition;
    }
}
