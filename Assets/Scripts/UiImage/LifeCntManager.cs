using UnityEngine;

public class LifeCntManager : MonoBehaviour
{
    public LifeCntScaler[] lifeCntScalers; // LifeCntScaler 배열 (UI 생명력 바)

    void Start()
    {
        // ⭐ 자식 오브젝트에서 LifeCntScaler 전부 찾아서 배열에 저장 ⭐
        lifeCntScalers = GetComponentsInChildren<LifeCntScaler>();
    }

    public void RemoveOneLife(int _Cnt)
    {
        lifeCntScalers[lifeCntScalers.Length - _Cnt].ShrinkToZero(); // 받아온 정수에 맞는 라이프 배열 사이즈를 0으로 만든다.
    }

    public void StartHealOneLife(float _duration, int _Cnt)
    {
        lifeCntScalers[lifeCntScalers.Length - _Cnt].ExpandToOriginalSize(_duration); // 그리고 사이즈를 다시 키운다.
    }
}
