using System.Collections.Generic;
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
            IndividualSpawn(c);
        }
    }

    public void IndividualDelete(BattleChar c)
    {
        foreach(LifeBar l in lifeBars)
        {
            if(l.character == c)
            {
                lifeBars.Remove(l);
                Destroy(l.gameObject);
                break;
            }
        }
    }

    public void IndividualSpawn(BattleChar c)
    {
        GameObject bar = Instantiate(life_bar, transform);
        bar.transform.SetParent(this.gameObject.transform);
        LifeBar life = bar.GetComponent<LifeBar>();
        life.SetCharacter(c);
        lifeBars.Add(life);
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
