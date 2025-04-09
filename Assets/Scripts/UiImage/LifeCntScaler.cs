using UnityEngine;
using System.Collections;

public class LifeCntScaler : MonoBehaviour
{
    private Vector2 originalSize; // UI�� ���� ũ�� ����
    private RectTransform rectTransform; // RectTransform ����
    public bool onRecovered = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>(); // ���� UI�� RectTransform ��������
        originalSize = rectTransform.sizeDelta; // �ʱ� ũ�� ����
        ExpandToOriginalSize(0.1f);
    }

    // UI�� 0 ũ�⿡�� ���� ũ��� ���� Ŀ���� �ϴ� �Լ�
    public void ExpandToOriginalSize(float duration)
    {
        StartCoroutine(AnimateSize(Vector2.zero, originalSize, duration));
    }

    // UI ũ�⸦ ��� 0���� �����ϴ� �Լ�
    public void ShrinkToZero()
    {
        rectTransform.sizeDelta = Vector2.zero;
    }

    // ũ�� ��ȭ�� �ε巴�� �ִϸ��̼��ϴ� �ڷ�ƾ
    private IEnumerator AnimateSize(Vector2 from, Vector2 to, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration) // duration ���� �ִϸ��̼� ����
        {
            elapsedTime += Time.deltaTime; // ��� �ð� ������Ʈ
            rectTransform.sizeDelta = Vector2.Lerp(from, to, elapsedTime / duration); // ũ�� ����
            yield return null; // ���� �����ӱ��� ���
        }

        rectTransform.sizeDelta = to; // ���� ũ�� ����
    }
}
