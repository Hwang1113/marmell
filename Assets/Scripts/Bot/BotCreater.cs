using UnityEngine;

public class BotCreater : MonoBehaviour
{
    public GameObject botPrefab;           // 생성할 봇의 프리팹
    public int botNum = 10;                // 생성할 봇의 수 (Unity Inspector에서 관리)
    public Vector3 Location = Vector3.zero; // 봇들이 생성될 중심 위치 (Unity Inspector에서 관리)
    public float minDistance = 1f;         // 봇 간의 최소 거리 (Unity Inspector에서 관리)
    public float spawnAreaSize = 5f;       // 봇들이 생성될 범위 크기 (기본적으로 X, Z 축으로 지정된 크기)

    private void Start()
    {
        CreateBots();
    }

    private void CreateBots()
    {
        Vector3[] positions = new Vector3[botNum];  // 생성된 봇들의 위치를 저장할 배열

        // botNum만큼 봇을 생성
        for (int i = 0; i < botNum; i++)
        {
            // 유효한 위치를 찾을 때까지 반복
            Vector3 newPos = GetRandomPosition(positions, i);

            // 위치에 봇 인스턴스를 생성
            Instantiate(botPrefab, newPos, Quaternion.identity);
        }
    }

    private Vector3 GetRandomPosition(Vector3[] existingPositions, int currentIndex)
    {
        Vector3 newPosition = Vector3.zero; // newPosition을 초기화
        bool validPosition = false;

        while (!validPosition)
        {
            // 중심 위치를 기준으로 랜덤 위치 생성
            newPosition = Location + new Vector3(Random.Range(-spawnAreaSize, spawnAreaSize), 0, Random.Range(-spawnAreaSize, spawnAreaSize));

            validPosition = true;

            // 이미 생성된 봇들과 겹치지 않도록 확인
            for (int i = 0; i < currentIndex; i++)
            {
                if (Vector3.Distance(newPosition, existingPositions[i]) < minDistance)
                {
                    validPosition = false;
                    break;
                }
            }

            // 겹치지 않으면 해당 위치를 저장
            if (validPosition)
            {
                existingPositions[currentIndex] = newPosition;
            }
        }

        return newPosition;
    }
}
