using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MaterialSwitcherController : MonoBehaviour
{
    // 여러 개의 MaterialSwitcher를 관리할 리스트
    public List<MaterialSwitcher> materialSwitchers;

    // 현재 교체할 MaterialSwitcher의 인덱스 (순차적으로 바꾸기 위해)
    private int currentSwitcherIndex = 0;

    // 상태: 모두 바꾸기/순차적으로 바꾸기 선택
    public bool switchAll = true;

    void Start()
    {
        // MaterialSwitcher들이 할당되었는지 확인
        if (materialSwitchers == null || materialSwitchers.Count == 0)
        {
            materialSwitchers = GetComponentsInChildren<MaterialSwitcher>().ToList();
        }

        // 각 MaterialSwitcher의 이벤트를 구독
        foreach (var switcher in materialSwitchers)
        {
            switcher.OnMaterialChanged += HandleMaterialChanged;
        }
    }

    void Update()
    {
        // 예시: "N" 키를 눌렀을 때 실행
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (switchAll)
            {
                SwitchAllMaterials(); // 모두 바꾸는 기능 호출
            }
            else
            {
                SwitchNextMaterial(); // 순차적으로 하나씩 바꾸는 기능 호출
            }
        }
    }

    // 모두 바꾸는 메서드
    public void SwitchAllMaterials()
    {
        foreach (var switcher in materialSwitchers)
        {
            switcher.SwitchMaterials(); // 모든 switcher의 메터리얼을 변경
        }
    }

    // 순차적으로 하나씩 MaterialSwitcher의 메터리얼을 교체하는 메서드
    public void SwitchNextMaterial()
    {
        // 현재 인덱스에 해당하는 MaterialSwitcher가 있을 경우에만 교체
        if (materialSwitchers.Count > 0)
        {
            materialSwitchers[currentSwitcherIndex].SwitchMaterials(); // 현재 switcher의 메터리얼 교체

            // 인덱스를 다음으로 이동
            currentSwitcherIndex = (currentSwitcherIndex + 1) % materialSwitchers.Count;
        }
    }

    // 개별 MaterialSwitcher의 메터리얼 변경을 처리하는 이벤트 핸들러
    private void HandleMaterialChanged(MaterialSwitcher switcher)
    {
        Debug.Log($"{switcher.gameObject.name}의 메터리얼이 변경되었습니다!");
        // 추가적인 로직 처리 가능
    }

    // 종료 시 이벤트 구독 해제
    void OnDestroy()
    {
        // 이벤트 구독 해제
        foreach (var switcher in materialSwitchers)
        {
            switcher.OnMaterialChanged -= HandleMaterialChanged;
        }
    }
}
