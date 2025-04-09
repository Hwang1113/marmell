using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위한 네임스페이스

public class ComboTxt : MonoBehaviour
{
    private TextMeshProUGUI comboText; // TextMeshProUGUI 컴포넌트를 저장할 변수
    private int combo = 0; // 점수 변수
    private float disappearTime = 2f;
    private float curTime = 2f;
        
    void Start()
    {
        // TextMeshProUGUI 컴포넌트를 가져와서 scoreText에 할당
        comboText = GetComponent<TextMeshProUGUI>();
        if (comboText == null)
        {
            Debug.LogError("TextMeshProUGUI component not found on this object!");
        }

        // 점수를 초기화
        UpdateComboText();
        comboText.enabled = false;
    }

    void Update()
    {
        if (curTime >= disappearTime)
        {
            comboText.enabled = false;
            combo = 0;
        }
        else
        {
            curTime += Time.deltaTime;
        }
    }

    // 콥보를 갱신하고 텍스트를 업데이트하는 메서드
    public void UpdateCombo()
    {
        combo++;
        UpdateComboText();
    }

    // TextMeshProUGUI에 콤보를 갱신하는 메서드
    private void UpdateComboText()
    {
        if (comboText)
        {
            comboText.enabled = true;
            comboText.text = combo + "Combo".ToString(); // 점수를 텍스트로 갱신
            curTime = 0f;
        }
    }
}
