using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위한 네임스페이스
public class MidTxt : MonoBehaviour
{
    private TextMeshProUGUI MidText;
    private string curTxt;
    void Start()
    {
        MidText = GetComponent<TextMeshProUGUI>();
        if (!MidText)
        {
            Debug.LogError("TextMeshProUGUI component not found on this object!");
        }
        HideText();
    }

    public void UpdateText(string _text)
    {
        if (MidText)
        {
            MidText.text = _text; // Text 표기
        }
    }
    public void ShowText()
    {
        if (MidText)
        {
            MidText.color = new Color(MidText.color.r, MidText.color.g, MidText.color.b, 1);
        }
    }
    public void HideText()
    {
        if (MidText)
        {
            MidText.color = new Color(MidText.color.r, MidText.color.g, MidText.color.b, 0);
        }
    }
}
