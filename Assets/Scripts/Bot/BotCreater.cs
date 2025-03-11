using UnityEngine;

public class BotCreater : MonoBehaviour
{
    public GameObject botPrefab;           // ������ ���� ������
    public int botNum = 10;                // ������ ���� �� (Unity Inspector���� ����)
    public Vector3 Location = Vector3.zero; // ������ ������ �߽� ��ġ (Unity Inspector���� ����)
    public float minDistance = 1f;         // �� ���� �ּ� �Ÿ� (Unity Inspector���� ����)
    public float spawnAreaSize = 5f;       // ������ ������ ���� ũ�� (�⺻������ X, Z ������ ������ ũ��)

    private void Start()
    {
        CreateBots();
    }

    private void CreateBots()
    {
        Vector3[] positions = new Vector3[botNum];  // ������ ������ ��ġ�� ������ �迭

        // botNum��ŭ ���� ����
        for (int i = 0; i < botNum; i++)
        {
            // ��ȿ�� ��ġ�� ã�� ������ �ݺ�
            Vector3 newPos = GetRandomPosition(positions, i);

            // ��ġ�� �� �ν��Ͻ��� ����
            Instantiate(botPrefab, newPos, Quaternion.identity);
        }
    }

    private Vector3 GetRandomPosition(Vector3[] existingPositions, int currentIndex)
    {
        Vector3 newPosition = Vector3.zero; // newPosition�� �ʱ�ȭ
        bool validPosition = false;

        while (!validPosition)
        {
            // �߽� ��ġ�� �������� ���� ��ġ ����
            newPosition = Location + new Vector3(Random.Range(-spawnAreaSize, spawnAreaSize), 0, Random.Range(-spawnAreaSize, spawnAreaSize));

            validPosition = true;

            // �̹� ������ ����� ��ġ�� �ʵ��� Ȯ��
            for (int i = 0; i < currentIndex; i++)
            {
                if (Vector3.Distance(newPosition, existingPositions[i]) < minDistance)
                {
                    validPosition = false;
                    break;
                }
            }

            // ��ġ�� ������ �ش� ��ġ�� ����
            if (validPosition)
            {
                existingPositions[currentIndex] = newPosition;
            }
        }

        return newPosition;
    }
}
