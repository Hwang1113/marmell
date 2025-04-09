using UnityEngine;
using System.Collections;

public class LifeCntScaler : MonoBehaviour
{
    private Vector2 originalSize; // UI의 원래 크기 저장
    private RectTransform rectTransform; // RectTransform 참조
    public bool onRecovered = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>(); // 현재 UI의 RectTransform 가져오기
        originalSize = rectTransform.sizeDelta; // 초기 크기 저장
        ExpandToOriginalSize(0.1f);
    }

    // UI를 0 크기에서 원래 크기로 점점 커지게 하는 함수
    public void ExpandToOriginalSize(float duration)
    {
        StartCoroutine(AnimateSize(Vector2.zero, originalSize, duration));
    }

    // UI 크기를 즉시 0으로 설정하는 함수
    public void ShrinkToZero()
    {
        rectTransform.sizeDelta = Vector2.zero;
    }

    // 크기 변화를 부드럽게 애니메이션하는 코루틴
    private IEnumerator AnimateSize(Vector2 from, Vector2 to, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration) // duration 동안 애니메이션 실행
        {
            elapsedTime += Time.deltaTime; // 경과 시간 업데이트
            rectTransform.sizeDelta = Vector2.Lerp(from, to, elapsedTime / duration); // 크기 변경
            yield return null; // 다음 프레임까지 대기
        }

        rectTransform.sizeDelta = to; // 최종 크기 보정
    }
}
