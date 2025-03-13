using UnityEngine;
using System.Collections.Generic;

public class BotCreator : MonoBehaviour
{
    public GameObject botPrefab;           // 생성할 봇의 프리팹
    public int botNum = 10;                // 생성할 봇의 수 (Unity Inspector에서 관리)
    public Vector3 Location = Vector3.zero; // 봇들이 생성될 중심 위치 (Unity Inspector에서 관리)
    public float minDistance = 1f;         // 봇 간의 최소 거리 (Unity Inspector에서 관리)
    public float spawnAreaSize = 5f;       // 봇들이 생성될 범위 크기 (기본적으로 X, Z 축으로 지정된 크기)

    private List<GameObject> spawnedBots = new List<GameObject>();  // 생성된 봇들을 저장할 리스트
    private Queue<GameObject> botPool = new Queue<GameObject>();     // 객체 풀

    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>(); // 봇들이 차지한 위치를 저장 (중복 방지)

    void Start()
    {
        // 객체 풀 초기화
        InitializeObjectPool();

        // 봇 생성
        CreateBots();
    }

    // 객체 풀 초기화
    private void InitializeObjectPool()
    {
        // 객체 풀에 봇 프리팹을 미리 생성하여 넣기
        for (int i = 0; i < botNum; i++)
        {
            GameObject bot = Instantiate(botPrefab);
            bot.SetActive(false);  // 처음에는 비활성화
            botPool.Enqueue(bot);  // 풀에 추가
        }
    }

    // 봇 생성
    private void CreateBots()
    {
        Vector3[] positions = new Vector3[botNum];  // 생성된 봇들의 위치를 저장할 배열

        // botNum만큼 봇을 생성
        for (int i = 0; i < botNum; i++)
        {
            // 유효한 위치를 찾을 때까지 반복
            Vector3 newPos = GetRandomPosition(i);

            // 객체 풀에서 비활성화된 봇을 가져와 활성화하고 위치 설정
            GameObject bot = botPool.Dequeue();
            bot.transform.position = newPos;
            bot.SetActive(true);  // 봇 활성화

            // 생성된 봇을 리스트에 추가
            spawnedBots.Add(bot);
        }
    }

    // 유효한 위치를 찾는 함수
    private Vector3 GetRandomPosition(int currentIndex)
    {
        Vector3 newPosition;
        bool validPosition = false;

        // 유효한 위치를 찾을 때까지 반복
        do
        {
            newPosition = Location + new Vector3(Random.Range(-spawnAreaSize, spawnAreaSize), 0, Random.Range(-spawnAreaSize, spawnAreaSize));

            validPosition = !occupiedPositions.Contains(newPosition);

            if (validPosition)
            {
                // 위치가 유효하면 occupiedPositions에 추가
                occupiedPositions.Add(newPosition);
            }

        } while (!validPosition);

        return newPosition;
    }

    // 생성된 봇들을 반환하는 함수 (게임 중 다른 스크립트에서 접근 가능)
    public List<GameObject> GetSpawnedBots()
    {
        return spawnedBots;
    }

    // 봇을 다시 풀로 되돌리기 (게임 종료 후 리소스 정리)
    public void ReturnBotToPool(GameObject bot)
    {
        bot.SetActive(false); // 비활성화 후 풀로 반환
        botPool.Enqueue(bot);
        // 봇 위치 반환
        occupiedPositions.Remove(bot.transform.position);
    }
}
