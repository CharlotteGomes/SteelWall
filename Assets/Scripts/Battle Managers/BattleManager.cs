using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
public enum TurnStep
{
    ChooseSkill,
    ChooseTarget,
    MoveAction,
    ChooseSwapTarget,
    SkipTurn,
    ExecutingSkill,
    Finish
}

public class BattleManager : MonoBehaviour
{
    
    public BattleHud hud;

    [SerializeField] List<BattleChar> initiative = new List<BattleChar>();
    [SerializeField] GameObject pj_prefab, enemy_prefab, tomb_prefab;

    [FormerlySerializedAs("pjs")]
    [SerializeField] List<BattleCharSO> PlayerTest, enemy;

    [SerializeField] BattleChar[] pj_slot = new BattleChar[4];
    [SerializeField] BattleChar[] enemy_slot = new BattleChar[4];
    [SerializeField] List<BattleChar> test = new List<BattleChar>();

    public BattleChar current { get; private set; }

    public int turn { get; private set; } = 1;
    [SerializeField] int initiative_index = 0;
    bool battleEnded;
    private void Awake()
    {
        
    }

    void Start()
    {
        if (PlayerTest == null || enemy == null)
        {
            Debug.LogError("BattleManager character data lists are not assigned.", this);
            return;
        }

        List<PJData> pjs_data = new();
        List<EnemyData> enemy_data = new();
        foreach (BattleCharSO player in PlayerTest)
        {
            if (player is not PJSO playerData)
            {
                Debug.LogError("A player entry is missing or is not a PJSO asset.", this);
                return;
            }
            pjs_data.Add(new PJData(playerData));
        }
        foreach (BattleCharSO enemyAsset in enemy)
        {
            if (enemyAsset is not EnemySO enemyData)
            {
                Debug.LogError("An enemy entry is missing or is not an EnemySO asset.", this);
                return;
            }
            enemy_data.Add(new EnemyData(enemyData));
        }
        StartBattle(pjs_data, enemy_data);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartBattle(List<PJData> pjs, List<EnemyData> enemies)
    {
        if (pjs == null || enemies == null || pjs.Count == 0 || enemies.Count == 0)
        {
            Debug.LogError("Battle requires at least one player and one enemy.", this);
            return;
        }
        if (pjs.Count > pj_slot.Length || enemies.Count > enemy_slot.Length)
        {
            Debug.LogError($"Battle formation supports at most {pj_slot.Length} characters per side.", this);
            return;
        }
        if (hud == null || hud.lifeManager == null || pj_prefab == null || enemy_prefab == null || tomb_prefab == null)
        {
            Debug.LogError("BattleManager has missing HUD or prefab references.", this);
            return;
        }

        Array.Clear(pj_slot, 0, pj_slot.Length);
        Array.Clear(enemy_slot, 0, enemy_slot.Length);
        initiative.Clear();
        initiative_index = 0;
        turn = 1;
        battleEnded = false;
        SpawnCharacters(pjs, enemies);

        BattleChar firstPlayer = initiative.OfType<PjController>().FirstOrDefault();
        BattleChar firstEnemy = initiative.OfType<EnemyController>().FirstOrDefault();
        if (firstPlayer == null || firstEnemy == null)
        {
            Debug.LogError("Battle could not initialize both teams. Check character prefabs and data.", this);
            battleEnded = true;
            return;
        }
        hud.SetCharacters(firstPlayer, firstEnemy);
        hud.lifeManager.SpawnBars(initiative);
        StartCoroutine(TurnOrder());
    }

    public void EndBattle()
    {
        if (battleEnded) return;
        battleEnded = true;
        Debug.Log("Battle finished.", this);
    }

    bool TryFinishBattle()
    {
        bool hasPlayers = initiative.Any(character => character is PjController && character.CheckAlive());
        bool hasEnemies = initiative.Any(character => character is EnemyController && character.CheckAlive());
        if (hasPlayers && hasEnemies) return false;
        Debug.Log(!hasPlayers && !hasEnemies ? "Battle ended in a draw." :
            hasPlayers ? "Player team won." : "Enemy team won.", this);
        EndBattle();
        return true;
    }

    IEnumerator TurnOrder()
    {
        while (!battleEnded)
        {
            yield return StartCoroutine(CheckCharactersAlive());
            if (TryFinishBattle()) yield break;
            if (initiative.Count == 0) { EndBattle(); yield break; }
            if (initiative_index < 0 || initiative_index >= initiative.Count) initiative_index = 0;
            StartTurn();
            if (current == null) { EndBattle(); yield break; }
            yield return new WaitForSeconds(.5f);
            yield return StartCoroutine(current.OnGoingTurn());
            if (battleEnded) yield break;
            EndTurn();
            yield return StartCoroutine(SyncVisualPositions());
        }
    }

    void StartTurn()
    {
        if (initiative.Count == 0) { current = null; return; }
        current = initiative[initiative_index];
        hud.OnStartTurn(current);
        current.SetRound();
        RefreshHUD();
    }

    void EndTurn()
    {
        if (current != null)
        {
            current.OnEndTurn();
            hud.UnsubscribeEvents(current);
        }

        initiative_index++;
        if (initiative_index >= initiative.Count)
        {
            initiative_index = 0;
            turn++;

            // A round is complete only after every remaining combatant has acted.
            foreach (BattleChar character in initiative.ToList())
                if (character != null && character.CheckAlive()) character.OnRoundEnd();
        }

        RefreshHUD();
    }

    void SpawnCharacters(List<PJData> pjs, List<EnemyData> enemies)
    {
        for (int i = 0; i < pjs.Count; i++)
        {
            GameObject pj = Instantiate(pj_prefab);
            PjController ctrl = pj.GetComponent<PjController>();
            InitiateCharacter(ctrl, pjs[i]);
            PlaceCharacters(ctrl);
        }

        for(int i = 0; i < enemies.Count; i++)
        {
            GameObject n = Instantiate(enemy_prefab);
            EnemyController ctrl = n.GetComponent<EnemyController>();
            InitiateCharacter(ctrl, enemies[i]);
            PlaceCharacters(ctrl);
        }

        SetInitiative();
        SetCharacterSpriteRender();
        SetContext();
        SetAllPosition();
    }

    IEnumerator CheckCharactersAlive()
    {
        List<BattleChar> deadCharacters = initiative
            .Where(character => character != null && character is not BattleObjects && !character.CheckAlive())
            .ToList();
        foreach (BattleChar character in deadCharacters)
            yield return StartCoroutine(CharacterDeath(character));

        if (initiative.Count == 0) initiative_index = 0;
        else initiative_index = Mathf.Clamp(initiative_index, 0, initiative.Count - 1);
    }

    public void RefreshHUD()
    {
        if (hud != null) hud.RefreshHUD();
    }

    void SetCharacterSpriteRender()
    {
        for (int i = 0; i < pj_slot.Length; i++) 
        {
            if (pj_slot[i] == null) continue;
            BattleChar pj = pj_slot[i];
            pj.sprite_renderer.sortingOrder = 100 + (-1 * i); 
        }

        for (int i = 0; i < enemy_slot.Length; i++)
        {
            if (enemy_slot[i] == null) continue;
            BattleChar n = enemy_slot[i];
            n.sprite_renderer.sortingOrder = 100 + (-1 * i);
            n.sprite_renderer.flipX =true;
        }
    }


    public void PlaceCharacters(BattleChar character)
    {
        if (character == null) return;
        BattleChar[] slots = character is EnemyController ? enemy_slot : pj_slot;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null) continue;
            slots[i] = character;
            return;
        }
        Debug.LogError($"No free formation slot for {character.name}.", this);
    }

    public bool CheckOccupyPos(BattleChar[] list, int pos)
    {
        return list[pos] != null;
    }

    void SetInitiative()
    {
        initiative = initiative.OrderByDescending(a => a.GetStat(StatType.SPD)).ToList();   
    }

    void SetContext()
    {
        List<BattleChar> playerFormation = pj_slot.ToList();
        List<BattleChar> enemyFormation = enemy_slot.ToList();
        for (int i = 0; i < pj_slot.Length; i++)
            if (pj_slot[i] != null) pj_slot[i].CreateContext(playerFormation, enemyFormation, i);
        for (int i = 0; i < enemy_slot.Length; i++)
            if (enemy_slot[i] != null) enemy_slot[i].CreateContext(enemyFormation, playerFormation, i);
    }

    void SetAllPosition()
    {
        for (int i = 0; i < pj_slot.Length; i++)
            if (pj_slot[i] != null) pj_slot[i].transform.position = GetWorldPos(i, pj_slot[i]);
        for (int i = 0; i < enemy_slot.Length; i++)
            if (enemy_slot[i] != null) enemy_slot[i].transform.position = GetWorldPos(i, enemy_slot[i]);
    }

    Vector3 GetWorldPos(int i, BattleChar c)
    {
        BattleChar[] battleChar = enemy_slot.Where(b => b == c).ToArray();
        if (battleChar.Length > 0)
        {
            return (Vector3)(i * 2.5f * Vector2.right)
                + new Vector3(3, -1.25f, -5);
        }

        return (Vector3)(i * 2.5f * Vector2.left)
            + new Vector3(-3, -1.25f, -5);
    }

    void InitiateCharacter(BattleChar character, BattleCharData data)
    {
        if (character == null || data == null || character.sprite_renderer == null)
        {
            Debug.LogError("Character prefab is missing its controller, SpriteRenderer, or character data.", this);
            return;
        }
        character.SetBattleChar(data);
        character.SetBattleManager(this);
        character.InitStats();
        initiative.Add(character);
    }

    IEnumerator SyncVisualPositions()
    {
        List<Coroutine> movements = new List<Coroutine>();
        List<BattleChar> off = new List<BattleChar>();
        for (int i = 0; i < pj_slot.Length; i++)
        {
            if (pj_slot[i] != null)
            {
                if (off.Contains(pj_slot[i])) continue;
                Vector2 targetPos = GetWorldPos(i, pj_slot[i]);

                movements.Add(StartCoroutine(MoveToPosition(pj_slot[i].transform, targetPos)));
                off.Add(pj_slot[i]);
            }
        }

        for (int i = 0; i < enemy_slot.Length; i++)
        {
            if (enemy_slot[i] != null)
            {
                if (off.Contains(enemy_slot[i])) continue;
                Vector2 targetPos = GetWorldPos(i, enemy_slot[i]);

                movements.Add(StartCoroutine(MoveToPosition(enemy_slot[i].transform, targetPos)));
                off.Add(enemy_slot[i]);
            }
        }

        foreach (var move in movements)
        {
            yield return move;
        }
    }

    IEnumerator MoveToPosition(Transform tf, Vector3 target)
    {
        Vector3 start = tf.position;
        float duration = 0.4f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;
            t = Mathf.SmoothStep(0f, 1f, t);

            tf.position = Vector3.Lerp(start, target, t);
            hud.lifeManager.MoveBars();
            yield return null;
        }

        tf.position = target;
    }


    public void SwapPosition(BattleChar character, int targetPosition)
    {
        if (character == null) return;
        BattleChar[] slots = character is PjController ? pj_slot :
            character is EnemyController ? enemy_slot : null;
        if (slots == null || targetPosition < 0 || targetPosition >= slots.Length) return;
        int currentPosition = Array.IndexOf(slots, character);
        if (currentPosition < 0 || currentPosition == targetPosition) return;
        BattleChar displaced = slots[targetPosition];
        slots[targetPosition] = character;
        slots[currentPosition] = displaced;
        SetCharacterSpriteRender();
        SetContext();
        SetAllPosition();
        RefreshHUD();
    }

    public void InitiativeRemove(BattleChar b)
    {
        int removedIndex = initiative.IndexOf(b);
        if (removedIndex < 0) return;
        initiative.RemoveAt(removedIndex);
        if (removedIndex < initiative_index) initiative_index--;
        if (initiative.Count == 0 || initiative_index >= initiative.Count) initiative_index = 0;
    }

    public void SlotRemove(BattleChar b)
    {
        for(int i = 0; i < 4; i++)
        {
            if(pj_slot[i] == b) pj_slot[i] = null;
            if(enemy_slot[i] == b) enemy_slot[i] = null;
        }
    }

    public void HudRemoveCharInfo(BattleChar c)
    {
        hud.lifeManager.IndividualDelete(c);
    }

    public int GetPosition(BattleChar b)
    {
        for (int i = 0; i < pj_slot.Length; i++)
        {
            if (pj_slot[i] == b)
                return i;
        }

        for (int i = 0; i < enemy_slot.Length; i++)
        {
            if (enemy_slot[i] == b)
                return i;
        }

        return -1;
    }

    IEnumerator CharacterDeath(BattleChar b)
    {
        InitiativeRemove(b);
        SlotRemove(b);
        HudRemoveCharInfo(b);
        if(b is not BattleObjects) { SpawnTomb(b); }
        SetContext();
        SetAllPosition();
        b.CharacterDeath();
        hud.lifeManager.MoveBars();
        yield return null;
    }

    void SpawnTomb(BattleChar b)
    {
        GameObject tomb = Instantiate(tomb_prefab);
        BattleTomb battleTomb = tomb.GetComponent<BattleTomb>();
        if (b is EnemyController)
        {
            enemy_slot[b.GetSelfPos()] = battleTomb;
            return;
        }

        pj_slot[b.GetSelfPos()] = battleTomb;
    }
}
