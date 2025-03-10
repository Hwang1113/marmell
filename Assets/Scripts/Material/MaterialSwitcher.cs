using UnityEngine;
using System.Collections.Generic;

public class MaterialSwitcher : MonoBehaviour
{
    // Skinned Mesh Renderer
    private SkinnedMeshRenderer skinnedMeshRenderer;

    // �� ���� ���͸����� ������ ����Ʈ
    private List<Material> materialList = new List<Material>();

    // ���� ������ ���͸����� ���� (0 �Ǵ� 1)
    private bool isMaterial1Active = true;

    // ���͸��� ��ü �̺�Ʈ ����
    public delegate void MaterialChangedHandler(MaterialSwitcher switcher);
    public event MaterialChangedHandler OnMaterialChanged;

    void Start()
    {
        // SkinnedMeshRenderer�� �ڵ����� �����ɴϴ�.
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

        if (skinnedMeshRenderer == null)
        {
            Debug.LogError("SkinnedMeshRenderer�� �� ������Ʈ�� �����ϴ�.");
            return;
        }

        // ���� ����� ���͸������ ����Ʈ�� �����մϴ�.
        materialList.Clear();
        materialList.AddRange(skinnedMeshRenderer.materials);

        // �ʱ� ���͸��� ���� (Start���� �ѹ��� ȣ��)
        UpdateMaterials();
    }

    // ���͸��� ��ü �Լ�
    public void SwitchMaterials()
    {
        // ���¸� ������Ű�� ���͸��� ������Ʈ
        isMaterial1Active = !isMaterial1Active;
        UpdateMaterials();

        // ���͸����� ��ü�� ��, �̺�Ʈ�� Ʈ�����մϴ�.
        OnMaterialChanged?.Invoke(this); // ���� switcher�� �̺�Ʈ�� ����
    }

    // ������ ���͸����� �����ϴ� �Լ�
    private void UpdateMaterials()
    {
        if (materialList.Count >= 2)
        {
            // �迭�� �ϳ��� ���͸��� �Ҵ�
            Material[] materials = new Material[1];

            // ���� ���õ� ���͸����� �迭�� ����
            if (isMaterial1Active)
            {
                materials[0] = materialList[0];
            }
            else
            {
                materials[0] = materialList[1];
            }

            // ������Ʈ�� ���͸��� �迭�� SkinnedMeshRenderer�� �Ҵ�
            skinnedMeshRenderer.materials = materials;
        }
        else
        {
            Debug.LogError("���͸��� ����Ʈ�� ������� �ʽ��ϴ�. �ּ� 2���� ���͸����� �ʿ��մϴ�.");
        }
    }
}
