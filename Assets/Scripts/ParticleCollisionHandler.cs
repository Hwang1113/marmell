using UnityEngine;
using System.Collections.Generic;

public class ParticleCollisionHandler : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.CollisionModule collisionModule;
    public Terrain terrain;  // Terrain을 연결합니다.
    public float radius = 2f;  // 색상을 변경할 반경 (범위를 2로 설정, 필요에 따라 조정)
    private float[,,] originalAlphaMap; // 게임 종료 시 원래 알파맵 값을 저장하기 위한 변수

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        collisionModule = ps.collision;

        if (terrain == null)
        {
            terrain = Terrain.activeTerrain;  // Terrain이 없다면 기본 Terrain을 사용
        }

        // 게임 시작 시 알파맵의 원본 데이터를 저장합니다.
        TerrainData terrainData = terrain.terrainData;
        originalAlphaMap = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

        Debug.Log("Particle Collision Handler Initialized.");
    }

    void OnParticleCollision(GameObject other)
    {
        // 충돌 태그가 "Ground"인지 확인
        if (ps == null || other == null) return;

        if (other.CompareTag("Ground"))
        {
            // 충돌 이벤트 리스트
            List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
            ps.GetCollisionEvents(other, collisionEvents);

            // 충돌 이벤트에서 위치 정보 추출
            if (collisionEvents.Count == 0)
            {
                Debug.Log("No collisions detected.");
            }

            foreach (var collisionEvent in collisionEvents)
            {
                Vector3 collisionPoint = collisionEvent.intersection;

                // 알파맵 수정
                Debug.Log("Modifying Terrain AlphaMap at: " + collisionPoint);
                ModifyTerrainAlphaMap(collisionPoint);
            }
        }
    }

    void ModifyTerrainAlphaMap(Vector3 collisionPoint)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;

        // 충돌 지점을 Terrain 좌표계로 변환
        float normalizedX = (collisionPoint.x - terrainPos.x) / terrainData.size.x;
        float normalizedZ = (collisionPoint.z - terrainPos.z) / terrainData.size.z;

        // **수정된 변환**: x와 z를 교환
        int xBase = Mathf.FloorToInt(normalizedZ * terrainData.alphamapWidth);  // Z는 X로
        int zBase = Mathf.FloorToInt(normalizedX * terrainData.alphamapHeight); // X는 Z로

        // 디버그: 계산된 xBase, zBase 출력
        Debug.Log($"Base Coordinates (xBase, zBase): ({xBase}, {zBase})");

        // 알파맵의 경계를 벗어나지 않도록 유효한 범위로 제한
        int alphamapWidth = terrainData.alphamapWidth;
        int alphamapHeight = terrainData.alphamapHeight;

        xBase = Mathf.Clamp(xBase, 0, alphamapWidth - 1);
        zBase = Mathf.Clamp(zBase, 0, alphamapHeight - 1);

        // 디버그: 클램프된 xBase, zBase 출력
        Debug.Log($"Clamped Coordinates (xBase, zBase): ({xBase}, {zBase})");

        // 텍스처 맵을 가져오기
        float[,,] alphaMap = terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);

        // 충돌 지점 주변의 원형 영역만 알파맵 수정
        for (float x = -radius; x <= radius; x++)
        {
            for (float z = -radius; z <= radius; z++)
            {
                // 현재 좌표의 거리가 반경 내에 있으면 알파맵 수정
                if (x * x + z * z <= radius * radius)
                {
                    // 충돌 위치를 기준으로 범위 내 좌표로 변환
                    int xOffset = Mathf.Clamp(Mathf.FloorToInt(xBase + x), 0, alphamapWidth - 1);
                    int zOffset = Mathf.Clamp(Mathf.FloorToInt(zBase + z), 0, alphamapHeight - 1);

                    // 디버그: xOffset, zOffset 값 출력
                    Debug.Log($"Offset Coordinates (xOffset, zOffset): ({xOffset}, {zOffset})");

                    // Only modify the second layer (set others to 0)
                    alphaMap[xOffset, zOffset, 0] = 0f;  // Set first layer to 0
                    alphaMap[xOffset, zOffset, 1] = 1f;  // Set second layer to 1
                }
            }
        }

        // 수정된 알파 맵을 적용
        terrainData.SetAlphamaps(0, 0, alphaMap);
    }



    // 게임이 종료될 때 알파맵을 원래 상태로 리셋
    void OnApplicationQuit()
    {
        if (terrain != null)
        {
            TerrainData terrainData = terrain.terrainData;
            terrainData.SetAlphamaps(0, 0, originalAlphaMap); // 원래 상태로 복구
            Debug.Log("AlphaMap has been reset to its original state.");
        }
    }
}
