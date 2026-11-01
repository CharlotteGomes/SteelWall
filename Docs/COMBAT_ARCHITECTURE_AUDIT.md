# Combat Architecture Audit

Branch audited: `testando-integracao`

## Executive summary

The combat foundation is partially modularized, but the eight proposed stages are not complete. The current implementation must be migrated incrementally: several gameplay paths still depend on the legacy `SkillData`, `BattleContext`, `BattleResult`, and `BattleChar` APIs. Replacing these types in one pass would risk breaking player input, enemy turns, HUD bindings, ScriptableObject assets, and serialized scene/prefab references.

## Verified state

| Area | Current state | Gap |
|---|---|---|
| Skill definitions | `SkillSO` stores skill metadata, effect references, modifier references, targeting and hit flags | Definition and runtime execution are still coupled through `SkillData` |
| Skill runtime | `SkillData` stores caster context, target candidates, cloned `SkillEffectSO` objects and modified values | No dedicated `SkillExecution`; effects are instantiated/destroyed during preparation/disposal |
| Modifiers | `SkillModifiersSO.Prepare(SkillData)` modifies runtime clones | Modifiers remain coupled to `SkillData`; repeated preparation can rebuild effect instances |
| Targeting | `SkillData.SetTargets` filters living characters by side and position | Target validation is not centralized; `BattleChar.SelectTarget` also mutates `BattleContext.Target` |
| Resolution | `BattleChar.ResolveSkill` calculates miss, dodge and critical outcomes; `BattleResult` stores per-target results | Hit/critical/damage logic is still owned by `BattleChar`; no shared resolver pipeline |
| Effects | Effect ScriptableObjects execute against `BattleResult.hit` | Effects do not express target policies independently; mixed target routing is limited |
| Status effects | `StatusEffectData` has source/owner and turn callbacks; resistance checks exist | `stacks` is overloaded; duration, potency and stack policy are not modeled independently |
| Round lifecycle | `BattleChar.OnRoundEnd` exists and `BattleManager.EndTurn` invokes it after the initiative wraps | No explicit global round event/service |
| Descriptions | `SkillDescriptionBuilder` supports static skill and runtime `SkillData` descriptions | No per-effect descriptor contract or localization abstraction |
| Presentation | `BattleCombatEvents` and optional `BattlePresentationController` exist | Events are raised from the legacy skill flow; no dedicated character/VFX/audio/camera adapters or animation completion synchronization |
| Enemy AI | `EnemyBehaviourSO` has `behaviour` and `conditional` lists | `EnemyController` currently selects only from `behaviour`; `conditional` and `ConditionalSkillSO.Check()` have no consumers in the audited flow |
| Assets/tests | Existing assets and GUIDs are in place | No evidence yet of combat-focused automated tests or module assembly definitions |

## Important verified defects and risks

1. `SkillData.SetEffects` instantiates every effect asset and `Dispose` destroys those instances. This avoids mutating the source asset, but incurs Unity object lifecycle work per execution and keeps mutable ScriptableObjects as runtime state.
2. `SkillData.Prepare` calls `SetSkill`, which rebuilds effect instances before applying modifiers. Target selection calls `Prepare`; this couples target selection to effect allocation and modifier execution.
3. `MarkCritModifier`, `MarkDamageModifier`, and `MarkStacksModifier` inspect `BattleContext.Target`, while other skill data is held in `SkillData.targets`. This creates two target sources that can diverge.
4. `BleedEffect.OnStartTurn` uses `stacks` as both damage potency and remaining lifetime: it deals the current stack value, then decrements it. There is no independent duration field, so damage changes every turn and duration is implicitly tied to potency.
5. `BattleChar.ReceiveStatus` stacks effects by matching display name, not a stable definition identity or configurable stacking policy. Resistance and immunity are not represented as separate policies.
6. `ConditionalSkillSO.Check()` is parameterless and the audited enemy flow does not call it. The conditional list therefore currently has no runtime effect.
7. Presentation events are optional and null-safe, but animation triggers are fire-and-forget. They do not gate combat resolution, which is desirable for reliability; explicit animation timing should be introduced only as a presentation-side signal.

## Migration plan and safety gates

### Stage 1 — Skill execution foundation

- Add an immutable runtime execution context with caster, definition, validated target snapshot, modifiers and per-target results.
- Introduce a target resolver and an effect executor.
- Keep compatibility adapters for `SkillData`, `BattleContext`, and `BattleResult` until player HUD and AI callers are migrated.
- Move modifier output into execution-local values; never write to a `SkillSO`, effect asset, or modifier asset.
- Remove runtime `Instantiate/Destroy` of effect ScriptableObjects only after every concrete effect has a runtime representation.

Acceptance: two actors can execute the same skill concurrently without shared mutable state; a three-target skill produces three independent target resolutions; effects can route to distinct targets.

### Stage 2 — Status-effect instances

- Keep `StatusEffectSO` as immutable definition data.
- Introduce an instance with definition, source, owner, stack count, potency, remaining duration, tick timing, and stack policy.
- Add explicit policies for stack, refresh, replace, reject, resistance and immunity.
- Preserve legacy constructors/serialized fields through adapters until all effect assets and constructors are migrated.
- Define round-end dispatch once in the battle lifecycle.

Acceptance: potency does not change as duration counts down; duration expires on the specified tick; stacking/refreshing behavior is deterministic.

### Stage 3 — Character and AI responsibilities

- Extract stat calculation, status lifecycle and skill execution behind components/services while preserving `BattleChar` as the scene-facing facade.
- Give AI the same target validation and execution pipeline as player actions.
- Replace parameterless conditional checks with a context-aware evaluation contract and migrate all conditional assets before removing the old API.

Acceptance: player and AI resolve the same skill under identical combat state; conditional skills are evaluated and usage limits are enforced.

### Stage 4 — Description builder

- Add one descriptor per effect type.
- Generate mechanical values from the same execution preview/calculation inputs as combat.
- Separate narrative text, mechanical values and localization keys.

Acceptance: descriptions for multi-effect skills show the correct targets, ranges, duration and potency for the current actor state.

### Stage 5 — Presentation

- Keep presentation subscribers optional and downstream from combat events.
- Add adapters for character animation, VFX, audio and camera.
- Presentation callbacks may sequence visuals, but must never determine whether rules are applied.

Acceptance: missing Animator, camera, audio or VFX components do not block the combat result.

### Stage 6 — Asset organization

- Move assets only with their matching `.meta` files.
- Keep GUIDs unchanged and verify serialized references after each move.
- Introduce assembly definitions only after dependency boundaries are mapped; do not add them during gameplay refactoring.

### Stage 7 — Automated validation

- Add EditMode tests for target selection, modifiers, status policies and description output.
- Add PlayMode tests for turn lifecycle, death, presentation absence and HUD integration.
- Validate scene/prefab references and compile in the project's actual Unity editor version.

## Implementation guardrails

- Do not delete legacy APIs while serialized assets or callers still reference them.
- Do not move assets without their `.meta` files.
- Do not add assembly definitions until cyclic and cross-module dependencies are known.
- Do not mark a stage complete based only on source inspection. Require compilation and the stage's acceptance tests.
