using UnityEngine;

public class LifeBar : MonoBehaviour
{
    [SerializeField] BattleChar character;
    [SerializeField] LifeBarFill fill;
    [SerializeField] BackLifeBar backFill;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) UpdateBars();
    }

    public void SetCharacter(BattleChar character)
    {
        this.character = character;
        UpdateBars();
        SetPos();
        SetSize();
    }

    public void UpdateBars()
    {
        fill.UpdateBar(character);
        StartCoroutine(backFill.UpdateBar(character));
    }

    public void SetPos()
    {
        transform.position = character.transform.position + new Vector3(0, -.25f);
    }

    public void SetSize()
    {
        transform.localScale = new Vector2(((float)(1 + character.GetSize()) / 2), 1);
    }
}
