using UnityEngine;
using TMPro;
using System.Collections.Generic;
public enum ResourceType
{
    CASH,
    HONOR,
    WAR_DOCUMENTS,
    BLUEPRINTS
}

public class PlayerResourcesManager : MonoBehaviour
{
    public List<ResourcesSO> resources;
    [SerializeField] TMP_Text[] texts;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug
        if (Input.GetKeyDown(KeyCode.C))
        {
            for(int i = 0; i < resources.Count; i++)
            {
                texts[i].text = $"{resources[i].Value}";
            }
        }
    }
}
