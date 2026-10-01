using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SkillIcon : MonoBehaviour, IPointerClickHandler
{
    Image image;
    [SerializeField] PJBattleHUD panel;
    public SkillSO so { get; private set; }
    public bool select = false;
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        panel.SetSkill(so);
    }

    public void UpdateSkillSO(SkillSO skSO)
    {
        so = skSO;
    }

    public void UpdateSprite()
    {
        image.sprite = so.spr; 
    }

    private void OnEnable()
    {
        image = GetComponent<Image>();
    }
}
