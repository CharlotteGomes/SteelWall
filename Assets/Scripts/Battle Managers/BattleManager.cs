using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework.Constraints;
using NUnit.Framework.Interfaces;
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
    [SerializeField] GameObject pj_prefab, enemy_prefab;
    [SerializeField] List<PJSO> test1 = new List<PJSO>();
    [SerializeField] List<EnemySO> test2 = new List<EnemySO>();

    [SerializeField] BattleChar[] pj_slot = new BattleChar[4];
    [SerializeField] BattleChar[] enemy_slot = new BattleChar[4];
    [SerializeField] List<BattleChar> test = new List<BattleChar>();

    public BattleChar current { get; private set; }

    public int turn { get; private set; } = 1;
    [SerializeField] int initiative_index = 0;
    private void Awake()
    {
        
    }

    void Start()
    {
        StartBattle(test1, test2);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) { SwapPosition(pj_slot[0], pj_slot[1]);}
    }

    public void StartBattle(List<PJSO> pjs, List<EnemySO> enemies)
    {
        initiative_index = 0;
        SpawnCharacters(pjs, enemies);
        hud.SetCharacter(pj_slot[0], enemy_slot[0]);
        StartCoroutine(TurnOrder());
    }
    
    IEnumerator TurnOrder()
    {
        while (true)
        {
            StartTurn();
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(current.OnGoingTurn());
            EndTurn();
        }
    }

    void StartTurn()
    {
        current = initiative[initiative_index];
        current.SetRound();
        if(current is PjController pj) { hud.SetCharacter(pj, enemy_slot[0]); }
        if(current is EnemyController enemy) { hud.SetCharacter(pj_slot[0], enemy); }
        RefreshHUD();
    }

    void EndTurn()
    {
        initiative_index++;
        if (initiative_index > initiative.Count - 1) { initiative_index = 0; turn++; }
        StartCoroutine(SyncVisualPositions());
        RefreshHUD();
    }

    public void RefreshHUD()
    {
        hud.RefreshHUD();
    }

    void SpawnCharacters(List<PJSO> pjs, List<EnemySO> enemies)
    {
        for (int i = 0; i < pjs.Count; i++)
        {
            GameObject pj = Instantiate(pj_prefab);
            PjController ctrl = pj.GetComponent<PjController>();
            pj_slot[i] = ctrl; 
            ctrl.sprite_renderer.sortingOrder = -1 * i;
            pj.name = pjs[i].Name;

            ctrl.SetBattleChar(pjs[i]);
            initiative.Add(ctrl);
        }

        for(int i = 0; i < enemies.Count; i++)
        {
            GameObject n = Instantiate(enemy_prefab);
            EnemyController ctrl = n.GetComponent<EnemyController>();
            enemy_slot[i] = ctrl;
            ctrl.sprite_renderer.flipX = true;
            ctrl.sprite_renderer.sortingOrder = -1 * i;
            n.name = enemies[i].Name;

            
            ctrl.SetBattleChar(enemies[i]);
            initiative.Add(ctrl);
        }
        SetContext();
        SetAllPosition();
        InitiateCharacter();
        SetInitiative();
    }

    public void CurrentSetTarget(BattleChar b)
    {
        if (current is not PjController p) return;
        SkillSO so = p.skill_read;
        switch (b)
        {
            case PjController pj:
                if (current.CurrentStep() == TurnStep.ChooseTarget)
                {
                    for (int i = 0; i < so.pos.Length; i++)
                    {
                        if (so.pos[i] == GetPosition(pj) && !so.affect_enemy) { current.SelectTarget(i); current.ConfirmTarget(); }
                    }
                    break;
                }
                if(current.CurrentStep() == TurnStep.ChooseSwapTarget)
                {
                    int diff = (int)Mathf.Abs(GetPosition(b) - GetPosition(current));
                    if(diff == 1) { current.ConfirmSwapTarget(GetPosition(b)); }
                }
                break;
            case EnemyController enemy:
                if (current.CurrentStep() == TurnStep.ChooseTarget)
                {
                    for (int i = 0; i < so.pos.Length; i++)
                    {
                        if (so.pos[i] == GetPosition(enemy) && so.affect_enemy) { current.SelectTarget(i); current.ConfirmTarget(); }
                    }
                    break;
                }
                break;
        }
    }

    void SetInitiative()
    {
        initiative = initiative.OrderByDescending(a => a.GetStat(StatType.SPD)).ToList();   
    }

    void SetContext()
    {
        for(int i = 0; i < pj_slot.Length; i++)
        {
            if (pj_slot[i] != null)
            {
                pj_slot[i].CreateContext(pj_slot.Where(b => b != null).ToList(), enemy_slot.Where(b => b != null).ToList(), i);
            }
        }
        for (int i = 0; i < enemy_slot.Length; i++)
        {
            if (enemy_slot[i] != null)
            {
                enemy_slot[i].CreateContext(enemy_slot.Where(b => b != null).ToList(), pj_slot.Where(b => b != null).ToList(), i);
            }
        }
    }

    void SetAllPosition()
    {
        for(int i = 0; i < pj_slot.Length; i++)
        {
            if(pj_slot[i] != null) { pj_slot[i].transform.position = (Vector3)(i * 2.5f * Vector2.left) + new Vector3(-3, -1.25f, -5); ; }
        }
        for(int i = 0; i < enemy_slot.Length; i++)
        {
            if(enemy_slot[i] != null) { enemy_slot[i].transform.position = (Vector3)(i * 2.5f * Vector2.right) + new Vector3(3, -1.25f, -5); }
        }
    }

    void InitiateCharacter()
    {
        foreach(BattleChar b in initiative)
        {
            b.SetBattleManager(this);
            b.InitStats();
        }
    }

    IEnumerator SyncVisualPositions()
    {
        List<Coroutine> movements = new List<Coroutine>();

        for (int i = 0; i < pj_slot.Length; i++)
        {
            if (pj_slot[i] != null)
            {
                Vector3 targetPos = (Vector3)(i * 2.5f * Vector2.left) + new Vector3(-3, -1.25f, -5);

                movements.Add(StartCoroutine(MoveToPosition(pj_slot[i].transform, targetPos)));
            }
        }

        for (int i = 0; i < enemy_slot.Length; i++)
        {
            if (enemy_slot[i] != null)
            {
                Vector3 targetPos = (Vector3)(i * 2.5f * Vector2.right) + new Vector3(3, -1.25f, -5);

                movements.Add(StartCoroutine(MoveToPosition(enemy_slot[i].transform, targetPos)));
            }
        }

        yield return new WaitForSeconds(0.5f);
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

            yield return null;
        }

        tf.position = target;
    }

    public void SwapPosition(BattleChar a, BattleChar b)
    {
        int PosA = GetPosition(a);
        int PosB = GetPosition(b);
        int OrderA = a.sprite_renderer.sortingOrder;
        int OrderB = b.sprite_renderer.sortingOrder;
        a.sprite_renderer.sortingOrder = OrderB;
        b.sprite_renderer.sortingOrder = OrderA;
        if (a == null || b == null || PosA == -1|| PosB == -1) { return;}

        if(a is PjController && b is PjController)
        {
            pj_slot[PosA] = b;
            pj_slot[PosB] = a;
        }

        if(a is EnemyController && b is EnemyController)
        {
            enemy_slot[PosA] = b;
            enemy_slot[PosB] = a;
        }
        SetContext();
    }

    public void InitiativeRemove(BattleChar b)
    {
        if (initiative.Contains(b)) { initiative.Remove(b); }
    }

    public void SlotRemove(BattleChar b)
    {
        switch(b)
        {
            case PjController pj:
                for(int i = 0; i < pj_slot.Length; i++)
                {
                    if(pj_slot[i] == pj) { pj_slot[i] = null; break; }
                }
                break;
            case EnemyController enemy:
                for(int i = 0; i < enemy_slot.Length; i++)
                {
                    if(enemy_slot[i] == enemy) { enemy_slot[i] = null; break; }
                }
                break;
        }
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
        SetContext();
        yield return null;
    }

    void UpdatePos()
    {
    }
}
