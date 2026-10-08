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

    }

    public void StartBattle(List<PJSO> pjs, List<EnemySO> enemies)
    {
        initiative_index = 0;
        SpawnCharacters(pjs, enemies);
        hud.SetCharacters(pj_slot[0], enemy_slot[0]);   
        hud.lifeManager.SpawnBars(initiative);
        StartCoroutine(TurnOrder());
    }
    
    IEnumerator TurnOrder()
    {
        while (true)
        {
            StartTurn();
            yield return new WaitForSeconds(.5f);
            yield return StartCoroutine(current.OnGoingTurn());
            EndTurn();
            yield return StartCoroutine(SyncVisualPositions());
        }
    }

    void StartTurn()
    {
        current = initiative[initiative_index];
        hud.OnStartTurn(current);
        current.SetRound();
        RefreshHUD();
    }

    void EndTurn()
    {
        current.OnEndTurn();
        hud.UnsubscribeEvents(current);
        initiative_index++;
        if (initiative_index > initiative.Count - 1) { initiative_index = 0; turn++; }
        
        RefreshHUD(); 
        
    }

    

    void SpawnCharacters(List<PJSO> pjs, List<EnemySO> enemies)
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

    public void RefreshHUD()
    {
        hud.RefreshHUD();
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
        if (character is EnemyController)
        {
            for (int i = 0; i < enemy_slot.Length; i++)
            {
                if (CheckOccupyPos(enemy_slot, i)) continue;

                enemy_slot[i] = character;
                break;
            }
            return;
        }
        for (int i = 0; i < pj_slot.Length; i++)
        {
            if (CheckOccupyPos(pj_slot, i)) continue;

            pj_slot[i] = character;
            break;
        }
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
        List<BattleChar> off = new List<BattleChar>();
        for(int i = 0; i < pj_slot.Length; i++)
        {
            if (pj_slot[i] != null)
            {
                if (off.Contains(pj_slot[i])) continue;
                pj_slot[i].CreateContext(pj_slot.Where(b => b != null).ToList(), enemy_slot.Where(b => b != null).ToList(), i);
                off.Add(pj_slot[i]);
            }
        }
        for (int i = 0; i < enemy_slot.Length; i++)
        {
            if (enemy_slot[i] != null)
            {
                if (off.Contains(enemy_slot[i])) continue;
                enemy_slot[i].CreateContext(enemy_slot.Where(b => b != null).ToList(), pj_slot.Where(b => b != null).ToList(), i);
                off.Add(pj_slot[i]);
            }
        }
    }

    void SetAllPosition()
    {
        List<BattleChar> off = new List<BattleChar>();
        for (int i = 0; i < pj_slot.Length; i++)
        {
            if (off.Contains(pj_slot[i])) continue;
            if(pj_slot[i] != null) { pj_slot[i].transform.position = GetWorldPos(i, pj_slot[i]); }
            off.Add(pj_slot[i]);
        }
        for(int i = 0; i < enemy_slot.Length; i++)
        {
            if (off.Contains(enemy_slot[i])) continue;
            if (enemy_slot[i] != null) { enemy_slot[i].transform.position = GetWorldPos(i, enemy_slot[i]); }
            off.Add(enemy_slot[i]);
        }
    }

    Vector3 GetWorldPos(int i, BattleChar c)
    {
        if (c is EnemyController)
        {
            return (Vector3)(i * 2.5f * Vector2.right)
                + new Vector3(3, -1.25f, -5);
        }

        return (Vector3)(i * 2.5f * Vector2.left)
            + new Vector3(-3, -1.25f, -5);
    }

    void InitiateCharacter(BattleChar b, BattleCharSO so)
    {
        initiative.Add(b);
        b.SetBattleChar(so);
        b.SetBattleManager(this);
        b.InitStats();
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


    public void SwapPosition(BattleChar c, int pos)
    {
        if (c == null ) { return;}
        Debug.Log($"{c.name} se moveu");
        if(c is PjController)
        {
            List<BattleChar> pj = pj_slot.Where(x => x != c).ToList();
            pj.Insert(pos, c);
            pj_slot = pj.ToArray();
        }

        if(c is EnemyController)
        {
            List<BattleChar> n = enemy_slot.Where(x => x != c).ToList();
            n.Insert(pos, c);
            enemy_slot = n.ToArray();
        }
        SetCharacterSpriteRender();
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
