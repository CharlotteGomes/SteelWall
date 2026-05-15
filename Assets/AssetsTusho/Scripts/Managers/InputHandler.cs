using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance;
    public Vector2 MoveInput;
    public Vector2Int HUDInput;
    public bool InteractionPressed = false;
    public bool ConfirmAction = false;
    public bool CancelAction = false;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        
    }

    void Update()
    {
        MoveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if(Input.GetKeyDown(KeyCode.W)) { HUDInput = Vector2Int.up; }
        else if(Input.GetKeyDown(KeyCode.S)) { HUDInput = Vector2Int.down; }
        else if(Input.GetKeyDown(KeyCode.D)) { HUDInput = Vector2Int.right; }
        else if(Input.GetKeyDown(KeyCode.A)) { HUDInput = Vector2Int.left; }
        else { HUDInput = Vector2Int.zero; }
        InteractionPressed = Input.GetKeyDown(KeyCode.E);
        ConfirmAction = Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space);
        CancelAction = Input.GetKeyDown(KeyCode.Escape);
    }
}
