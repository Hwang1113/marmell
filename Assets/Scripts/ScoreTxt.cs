using UnityEngine;
using TMPro; // TextMeshPro�� ����ϱ� ���� ���ӽ����̽�

public class ScoreTxt : MonoBehaviour
{
    private TextMeshProUGUI scoreText; // TextMeshProUGUI ������Ʈ�� ������ ����
    private int score = 0; // ���� ����

    void Start()
    {
        // TextMeshProUGUI ������Ʈ�� �����ͼ� scoreText�� �Ҵ�
        scoreText = GetComponent<TextMeshProUGUI>();
        if (scoreText == null)
        {
            Debug.LogError("TextMeshProUGUI component not found on this object!");
        }

        // ������ �ʱ�ȭ
        UpdateScoreText();
    }

    // ������ �����ϰ� �ؽ�Ʈ�� ������Ʈ�ϴ� �޼���
    public void UpdateScore(int newScore)
    {
        score = newScore;
        UpdateScoreText();
    }

    // TextMeshProUGUI�� ������ �����ϴ� �޼���
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString(); // ������ �ؽ�Ʈ�� ����
        }
    }
}
