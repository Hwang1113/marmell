using UnityEngine;
using System.Collections.Generic;

public class BotCreator : MonoBehaviour
{
    public GameObject botPrefab;           // ������ ���� ������
    public int botNum = 10;                // ������ ���� �� (Unity Inspector���� ����)
    public Vector3 Location = Vector3.zero; // ������ ������ �߽� ��ġ (Unity Inspector���� ����)
    public float minDistance = 1f;         // �� ���� �ּ� �Ÿ� (Unity Inspector���� ����)
    public float spawnAreaSize = 5f;       // ������ ������ ���� ũ�� (�⺻������ X, Z ������ ������ ũ��)

    private List<GameObject> spawnedBots = new List<GameObject>();  // ������ ������ ������ ����Ʈ
    private Queue<GameObject> botPool = new Queue<GameObject>();     // ��ü Ǯ

    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>(); // ������ ������ ��ġ�� ���� (�ߺ� ����)

    void Start()
    {
        // ��ü Ǯ �ʱ�ȭ
        InitializeObjectPool();

        // �� ����
        CreateBots();
    }

    // ��ü Ǯ �ʱ�ȭ
    private void InitializeObjectPool()
    {
        // ��ü Ǯ�� �� �������� �̸� �����Ͽ� �ֱ�
        for (int i = 0; i < botNum; i++)
        {
            GameObject bot = Instantiate(botPrefab);
            bot.SetActive(false);  // ó������ ��Ȱ��ȭ
            botPool.Enqueue(bot);  // Ǯ�� �߰�
        }
    }

    // �� ����
    private void CreateBots()
    {
        Vector3[] positions = new Vector3[botNum];  // ������ ������ ��ġ�� ������ �迭

        // botNum��ŭ ���� ����
        for (int i = 0; i < botNum; i++)
        {
            // ��ȿ�� ��ġ�� ã�� ������ �ݺ�
            Vector3 newPos = GetRandomPosition(i);

            // ��ü Ǯ���� ��Ȱ��ȭ�� ���� ������ Ȱ��ȭ�ϰ� ��ġ ����
            GameObject bot = botPool.Dequeue();
            bot.transform.position = newPos;
            bot.SetActive(true);  // �� Ȱ��ȭ

            // ������ ���� ����Ʈ�� �߰�
            spawnedBots.Add(bot);
        }
    }

    // ��ȿ�� ��ġ�� ã�� �Լ�
    private Vector3 GetRandomPosition(int currentIndex)
    {
        Vector3 newPosition;
        bool validPosition = false;

        // ��ȿ�� ��ġ�� ã�� ������ �ݺ�
        do
        {
            newPosition = Location + new Vector3(Random.Range(-spawnAreaSize, spawnAreaSize), 0, Random.Range(-spawnAreaSize, spawnAreaSize));

            validPosition = !occupiedPositions.Contains(newPosition);

            if (validPosition)
            {
                // ��ġ�� ��ȿ�ϸ� occupiedPositions�� �߰�
                occupiedPositions.Add(newPosition);
            }

        } while (!validPosition);

        return newPosition;
    }

    // ������ ������ ��ȯ�ϴ� �Լ� (���� �� �ٸ� ��ũ��Ʈ���� ���� ����)
    public List<GameObject> GetSpawnedBots()
    {
        return spawnedBots;
    }

    // ���� �ٽ� Ǯ�� �ǵ����� (���� ���� �� ���ҽ� ����)
    public void ReturnBotToPool(GameObject bot)
    {
        bot.SetActive(false); // ��Ȱ��ȭ �� Ǯ�� ��ȯ
        botPool.Enqueue(bot);
        // �� ��ġ ��ȯ
        occupiedPositions.Remove(bot.transform.position);
    }
}
