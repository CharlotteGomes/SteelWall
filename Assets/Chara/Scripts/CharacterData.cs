using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    //ScriptableObject para cada personagem que terá no jogo e que apareça no dialogo, responsavel por mudar sua expressão caso necessário
    public string characterName;

    public Sprite normal;
    public Sprite happy;
    public Sprite angry;
    public Sprite sad;

    public Sprite GetExpression(Expression exp)
    {
        switch (exp)
        {
            case Expression.Happy: return happy;
            case Expression.Angry: return angry;
            case Expression.Sad: return sad;
            default: return normal;
        }
    }


}
