using UnityEngine;
using TMPro;
public class ShowText : MonoBehaviour
{
    TMP_Text tmp;
    string text;

    void Awake()
    {
        tmp = GetComponent<TMP_Text>();
    }

    public void UpdateTMP()
    {
        tmp.text = text;
    }
    public void UpdateText(string txt)
    {
        text = txt;
    }

    private void OnEnable()
    {
        tmp = GetComponent<TMP_Text>();
    }
}
