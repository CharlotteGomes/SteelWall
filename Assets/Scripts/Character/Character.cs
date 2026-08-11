using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Header("Identify")]
    public string char_name;
    public string description;

    [Header("World")]
    [SerializeField] protected Vector3 Spawnpoint;

    [Header("Appearence")]
    [SerializeField] protected Sprite sprite;
    [SerializeField] protected Sprite[] portraits;

    //State
    CharacterState state;
}
