using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster Instance;
    public PartyManager partyManager;
    public BattleController battleController;
    public GameStates CurrentState;

    public void ChangeState(GameStates state)
    {
        if(CurrentState != null)CurrentState.Exit();
        CurrentState = state;
        CurrentState.Enter();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        ChangeState(new Explorer(PlayerController.Instance));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) { ChangeState(new Combat());}
        if(CurrentState != null) CurrentState.Update();
    }
}
