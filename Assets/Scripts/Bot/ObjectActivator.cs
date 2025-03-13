using UnityEngine;
using System.Collections;
using System;

public class ObjectActivator : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Collider objectCollider;
    private Vector3 initialScale;

    private bool isActive = false;
    private float scaleLerpTime = 2f;

    // 외부에서 설정할 수 있는 콜백 액션
    public Action onScaleComplete;

    void Start()
    {
        InitializeComponents();
        SetInactiveState();
    }

    private void InitializeComponents()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        objectCollider = GetComponent<Collider>();
        initialScale = transform.localScale;
    }

    private void SetInactiveState()
    {
        meshRenderer.enabled = false;
        objectCollider.enabled = false;
        transform.localScale = Vector3.zero;
    }

    // 객체를 활성화하는 코루틴
    public void ActivateObject()
    {
        if (!isActive)
        {
            StartCoroutine(ScaleObject(Vector3.zero, initialScale, scaleLerpTime));
            SetActiveState(true);
        }
    }

    // 객체를 비활성화하는 코루틴
    public void DeactivateObject()
    {
        if (isActive)
        {
            StartCoroutine(ScaleObject(initialScale, Vector3.zero, scaleLerpTime));
            SetActiveState(false);
        }
    }

    private IEnumerator ScaleObject(Vector3 fromScale, Vector3 toScale, float duration)
    {
        float elapsedTime = 0f;
        transform.localScale = fromScale;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);
            transform.localScale = Vector3.Lerp(fromScale, toScale, lerpFactor);
            yield return null;
        }

        transform.localScale = toScale;

        // 스케일링 완료 후 외부에서 설정한 콜백 실행
        onScaleComplete?.Invoke();
    }

    private void SetActiveState(bool isActiveState)
    {
        meshRenderer.enabled = isActiveState;
        objectCollider.enabled = isActiveState;
        isActive = isActiveState;
    }
}
