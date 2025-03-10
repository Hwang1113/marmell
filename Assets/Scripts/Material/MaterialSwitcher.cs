using UnityEngine;
using System.Collections.Generic;

public class MaterialSwitcher : MonoBehaviour
{
    // Skinned Mesh Renderer
    private SkinnedMeshRenderer skinnedMeshRenderer;

    // 두 개의 메터리얼을 저장할 리스트
    private List<Material> materialList = new List<Material>();

    // 현재 적용할 메터리얼을 추적 (0 또는 1)
    private bool isMaterial1Active = true;

    // 메터리얼 교체 이벤트 정의
    public delegate void MaterialChangedHandler(MaterialSwitcher switcher);
    public event MaterialChangedHandler OnMaterialChanged;

    void Start()
    {
        // SkinnedMeshRenderer를 자동으로 가져옵니다.
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

        if (skinnedMeshRenderer == null)
        {
            Debug.LogError("SkinnedMeshRenderer가 이 오브젝트에 없습니다.");
            return;
        }

        // 현재 적용된 메터리얼들을 리스트에 저장합니다.
        materialList.Clear();
        materialList.AddRange(skinnedMeshRenderer.materials);

        // 초기 메터리얼 설정 (Start에서 한번만 호출)
        UpdateMaterials();
    }

    // 메터리얼 교체 함수
    public void SwitchMaterials()
    {
        // 상태를 반전시키고 메터리얼 업데이트
        isMaterial1Active = !isMaterial1Active;
        UpdateMaterials();

        // 메터리얼이 교체된 후, 이벤트를 트리거합니다.
        OnMaterialChanged?.Invoke(this); // 현재 switcher를 이벤트에 전달
    }

    // 실제로 메터리얼을 적용하는 함수
    private void UpdateMaterials()
    {
        if (materialList.Count >= 2)
        {
            // 배열에 하나의 메터리얼만 할당
            Material[] materials = new Material[1];

            // 현재 선택된 메터리얼을 배열에 적용
            if (isMaterial1Active)
            {
                materials[0] = materialList[0];
            }
            else
            {
                materials[0] = materialList[1];
            }

            // 업데이트된 메터리얼 배열을 SkinnedMeshRenderer에 할당
            skinnedMeshRenderer.materials = materials;
        }
        else
        {
            Debug.LogError("메터리얼 리스트가 충분하지 않습니다. 최소 2개의 메터리얼이 필요합니다.");
        }
    }
}
