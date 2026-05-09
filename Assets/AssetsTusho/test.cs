using UnityEngine;

public class test : MonoBehaviour, IInteraction
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Interact()
    {
        Debug.Log(gameObject.name);
    }

    public Transform GetTransform()
    {
        return transform;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
