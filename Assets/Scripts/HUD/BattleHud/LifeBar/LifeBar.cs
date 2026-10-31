using UnityEngine;

public class LifeBar : MonoBehaviour
{
    public BattleChar character { get; private set; }
    [SerializeField] LifeBarFill fill;
    [SerializeField] BackLifeBar backFill;
    Coroutine backFillRoutine;

    public void SetCharacter(BattleChar character)
    {
        this.character = character;
        UpdateBars();
        SetPos();
        SetSize();
    }

    public void UpdateBars()
    {
        if (character == null) return;
        if (fill != null) fill.UpdateBar(character);
        if (backFillRoutine != null) StopCoroutine(backFillRoutine);
        if (backFill != null) backFillRoutine = StartCoroutine(backFill.UpdateBar(character));
    }

    public void SetPos()
    {
        if (character != null) transform.position = character.transform.position + new Vector3(0, -.25f);
    }

    public void SetSize()
    {
        transform.localScale = Vector2.one;
    }

    private void OnDisable()
    {
        if (backFillRoutine != null)
        {
            StopCoroutine(backFillRoutine);
            backFillRoutine = null;
        }
    }
}