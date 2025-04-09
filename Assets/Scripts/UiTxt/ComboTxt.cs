using UnityEngine;
using TMPro; // TextMeshPro�� ����ϱ� ���� ���ӽ����̽�

public class ComboTxt : MonoBehaviour
{
    private TextMeshProUGUI comboText; // TextMeshProUGUI ������Ʈ�� ������ ����
    private int combo = 0; // ���� ����
    private float disappearTime = 2f;
    private float curTime = 2f;
        
    void Start()
    {
        // TextMeshProUGUI ������Ʈ�� �����ͼ� scoreText�� �Ҵ�
        comboText = GetComponent<TextMeshProUGUI>();
        if (comboText == null)
        {
            Debug.LogError("TextMeshProUGUI component not found on this object!");
        }

        // ������ �ʱ�ȭ
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

    // �ߺ��� �����ϰ� �ؽ�Ʈ�� ������Ʈ�ϴ� �޼���
    public void UpdateCombo()
    {
        combo++;
        UpdateComboText();
    }

    // TextMeshProUGUI�� �޺��� �����ϴ� �޼���
    private void UpdateComboText()
    {
        if (comboText)
        {
            comboText.enabled = true;
            comboText.text = combo + "Combo".ToString(); // ������ �ؽ�Ʈ�� ����
            curTime = 0f;
        }
    }
}
