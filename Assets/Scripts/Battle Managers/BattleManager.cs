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
        if (Input.GetKeyDown(KeyCode.Space)) { hud.lifeManager.SpawnBars(initiative); }
    }

    public void StartBattle(List<PJSO> pjs, List<EnemySO> enemies)
    {
        initiative_index = 0;
        SpawnCharacters(pjs, enemies);
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
        current.OnEndTurn();
        RefreshHUD(); 
        StartCoroutine(SyncVisualPositions());
        
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

    public bool CanOccupy(BattleChar character, int TargetPos)
    {
        int size = character.GetSize() - 1;

        if (character is EnemyController n)
        {
            if (TargetPos + size >= enemy_slot.Length) return false;
        }
        if (TargetPos + size >= pj_slot.Length) return false;
        return true;
    }

    public void PlaceCharacters(BattleChar character)
    {
        int size = character.GetSize();
        int j = 0;
        if (character is EnemyController)
        {
            for (int i = 0; i < enemy_slot.Length; i++)
            {
                if (j == size) break;
                if (CheckOccupyPos(enemy_slot, i)) continue;

                enemy_slot[i] = character;
                j++;
            }
            return;
        }
        for (int i = 0; i < pj_slot.Length; i++)
        {
            if (j == size) break;
            if (CheckOccupyPos(pj_slot, i)) continue;

            pj_slot[i] = character;
            j++;
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
    int size = c.GetSize();

    float center = i + (size - 1) / 2f;

    if (c is EnemyController)
    {
        return (Vector3)(center * 2.5f * Vector2.right)
            + new Vector3(3, -1.25f, -5);
    }

    return (Vector3)(center * 2.5f * Vector2.left)
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
            hud.lifeManager.MoveBars();
            yield return null;
        }

        tf.position = target;
    }


    public void SwapPosition(BattleChar c, int pos)
    {
        if (c == null ) { return;}

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
