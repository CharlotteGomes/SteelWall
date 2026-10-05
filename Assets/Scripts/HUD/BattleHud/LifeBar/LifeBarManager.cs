using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class LifeBarManager : MonoBehaviour
{
    [SerializeField] GameObject life_bar;
    [SerializeField] List<LifeBar> lifeBars = new List<LifeBar>();


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateLifeBars()
    {
        foreach(LifeBar life in lifeBars)
        {
            life.UpdateBars();
        }
    }

    public void SpawnBars(List<BattleChar> list)
    {
        foreach (BattleChar c in list)
        {
            GameObject bar = Instantiate(life_bar, transform);
            bar.transform.SetParent(this.gameObject.transform);
            LifeBar life = bar.GetComponent<LifeBar>();
            life.SetCharacter(c);
            lifeBars.Add(life);
        }
    }

    public void MoveBars()
    {
        foreach(LifeBar bar in lifeBars)
        {
            bar.SetPos();
        }
    }

    public void DeleteBars()
    {
        foreach(LifeBar bar in lifeBars)
        {
            Destroy(bar.gameObject);
        }
        lifeBars.Clear();
    }

    public void HideBars()
    {
        foreach(LifeBar bar in lifeBars)
        {
            bar.GetComponent<Image>().enabled = false;
        }
    }

    public void ShowBars() 
    {
        foreach(LifeBar bar in lifeBars)
        {
            bar.GetComponent<Image>().enabled = true;
        }
    }
}
