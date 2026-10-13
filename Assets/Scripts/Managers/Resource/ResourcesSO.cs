using UnityEngine;

[CreateAssetMenu(fileName = "ResourcesSO", menuName = "Scriptable Objects/Managers/ResourceManager/Resource")]
public class ResourcesSO : ScriptableObject
{
    public ResourceType resource;
    public string resource_name;
    [SerializeField] int value;
    public int Value => value;

    public void AddValue(ResourcesSO so)
    {
        if (so.resource != resource) { Debug.LogWarning("Tipo de recurso diferente"); return; }
        if(so.value < 0) { Debug.LogWarning("Valor negativo para soma de recursos"); return; }
        value += so.Value;
    }
}
