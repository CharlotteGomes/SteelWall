using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        SpawnCharacters(pjs, enemies);
        initiative_index = 0;
        StartCoroutine(TurnOrder());
    }
    
    IEnumerator TurnOrder()
    {
        while (true)
        {
            current = initiative[initiative_index];
            UpdateHud(current);
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(current.OnGoingTurn());
            yield return new WaitForSeconds(0.5f);
            initiative_index++;
            if (initiative_index > initiative.Count - 1) { initiative_index = 0; turn++; }
        }
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

    public void UpdateHud(BattleChar b)
    {
        if(b is PjController)
        {
            hud.pj_panel.DiselectAllSkillIcon();
            hud.pj_panel.SetCharacter(b);
            hud.pj_panel.UpdateChildren();
        }
        if (b is EnemyController)
        {
            hud.enemy_panel.SetCharacter(b);
            hud.enemy_panel.UpdateChildren();
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
            if(pj_slot[i] != null) { pj_slot[i].transform.position = (Vector3)(i * 2.5f * Vector2.left) + new Vector3(-3, 1f, -5); }
        }
        for(int i = 0; i < enemy_slot.Length; i++)
        {
            if(enemy_slot[i] != null) { enemy_slot[i].transform.position = (Vector3)(i * 2.5f * Vector2.right) + new Vector3(3, 1f, -5); }
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

    IEnumerator LerpSetPos(BattleChar a, BattleChar b)
    {
        Transform tfA = a.transform;
        Transform tfB = b.transform;
        Vector2 EndPosA = tfB.position;
        Vector2 EndPosB = tfA.position;
        Vector2 velocityA = Vector2.zero;
        Vector2 velocityB = Vector2.zero;
        float smoothness = .1f;
        float distA = Vector2.Distance(tfA.position, EndPosA);
        float distB = Vector2.Distance(tfB.position, EndPosB);
        while (distA > 0.1f || distB > 0.1f)
        {
            tfA.position = Vector2.SmoothDamp(tfA.position, EndPosA, ref velocityA, smoothness);
            tfB.position = Vector2.SmoothDamp(tfB.position, EndPosB, ref velocityB, smoothness);
            distA = Vector2.Distance(tfA.position, EndPosA);
            distB = Vector2.Distance(tfB.position, EndPosB);
            yield return null;
        }
        tfA.position = EndPosA;
        tfB.position = EndPosB;
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
        StartCoroutine(LerpSetPos(a, b));
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
