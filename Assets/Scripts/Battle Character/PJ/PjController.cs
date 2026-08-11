using UnityEngine;

public class PjController : BattleChar
{
    [SerializeField] SkillSO test;

    private void Awake()
    {
        ctx = new BattleContext(this);
        ctx.Target = this;
        InitStats();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            test.ExecuteSkill(ctx);
        }
    }
}
