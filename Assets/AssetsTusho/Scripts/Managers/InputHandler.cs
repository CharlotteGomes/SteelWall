using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance;
    public Vector2 MoveInput;
    public bool InteractionPressed = false;

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
        InteractionPressed = Input.GetKeyDown(KeyCode.E);
    }
}
