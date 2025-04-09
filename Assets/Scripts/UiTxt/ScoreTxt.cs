using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위한 네임스페이스

public class ScoreTxt : MonoBehaviour
{
    private TextMeshProUGUI scoreText; // TextMeshProUGUI 컴포넌트를 저장할 변수
    private int score = 0; // 점수 변수

    void Start()
    {
        // TextMeshProUGUI 컴포넌트를 가져와서 scoreText에 할당
        scoreText = GetComponent<TextMeshProUGUI>();
        if (!scoreText)
        {
            Debug.LogError("TextMeshProUGUI component not found on this object!");
        }

        // 점수를 초기화
        UpdateScoreText();
    }

    // 점수를 갱신하고 텍스트를 업데이트하는 메서드
    public void UpdateScore(int _newScore)
    {
        score = _newScore;
        UpdateScoreText();
    }

    // TextMeshProUGUI에 점수를 갱신하는 메서드
    private void UpdateScoreText()
    {
        if (!scoreText)
        {
            scoreText.text = "Score: " + score.ToString(); // 점수를 텍스트로 갱신
        }
    }
}
