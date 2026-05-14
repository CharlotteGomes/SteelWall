using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;    
    InputHandler Input;
    PlayerMove playerMove;
    public bool canControl = true;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Input = InputHandler.Instance;
        playerMove = GetComponent<PlayerMove>();
    }

    void Update()
    {
        if (!canControl) return;
        playerMove.Walk(Input.MoveInput);
    }
}
