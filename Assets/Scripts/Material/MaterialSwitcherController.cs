using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MaterialSwitcherController : MonoBehaviour
{
    // ���� ���� MaterialSwitcher�� ������ ����Ʈ
    public List<MaterialSwitcher> materialSwitchers;

    // ���� ��ü�� MaterialSwitcher�� �ε��� (���������� �ٲٱ� ����)
    private int currentSwitcherIndex = 0;

    // ����: ��� �ٲٱ�/���������� �ٲٱ� ����
    public bool switchAll = true;

    void Start()
    {
        // MaterialSwitcher���� �Ҵ�Ǿ����� Ȯ��
        if (materialSwitchers == null || materialSwitchers.Count == 0)
        {
            materialSwitchers = GetComponentsInChildren<MaterialSwitcher>().ToList();
        }

        // �� MaterialSwitcher�� �̺�Ʈ�� ����
        foreach (var switcher in materialSwitchers)
        {
            switcher.OnMaterialChanged += HandleMaterialChanged;
        }
    }

    void Update()
    {
        // ����: "N" Ű�� ������ �� ����
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (switchAll)
            {
                SwitchAllMaterials(); // ��� �ٲٴ� ��� ȣ��
            }
            else
            {
                SwitchNextMaterial(); // ���������� �ϳ��� �ٲٴ� ��� ȣ��
            }
        }
    }

    // ��� �ٲٴ� �޼���
    public void SwitchAllMaterials()
    {
        foreach (var switcher in materialSwitchers)
        {
            switcher.SwitchMaterials(); // ��� switcher�� ���͸����� ����
        }
    }

    // ���������� �ϳ��� MaterialSwitcher�� ���͸����� ��ü�ϴ� �޼���
    public void SwitchNextMaterial()
    {
        // ���� �ε����� �ش��ϴ� MaterialSwitcher�� ���� ��쿡�� ��ü
        if (materialSwitchers.Count > 0)
        {
            materialSwitchers[currentSwitcherIndex].SwitchMaterials(); // ���� switcher�� ���͸��� ��ü

            // �ε����� �������� �̵�
            currentSwitcherIndex = (currentSwitcherIndex + 1) % materialSwitchers.Count;
        }
    }

    // ���� MaterialSwitcher�� ���͸��� ������ ó���ϴ� �̺�Ʈ �ڵ鷯
    private void HandleMaterialChanged(MaterialSwitcher switcher)
    {
        Debug.Log($"{switcher.gameObject.name}�� ���͸����� ����Ǿ����ϴ�!");
        // �߰����� ���� ó�� ����
    }

    // ���� �� �̺�Ʈ ���� ����
    void OnDestroy()
    {
        // �̺�Ʈ ���� ����
        foreach (var switcher in materialSwitchers)
        {
            switcher.OnMaterialChanged -= HandleMaterialChanged;
        }
    }
}
