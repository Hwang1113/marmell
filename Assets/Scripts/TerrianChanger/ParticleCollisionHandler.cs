using UnityEngine;
using System.Collections.Generic;

public class ParticleCollisionHandler : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.CollisionModule collisionModule;
    public Terrain terrain;
    public float radius = 2f;
    private float[,,] originalAlphaMap; // 게임 종료 시 원래 알파 맵 값 저장용
    private TerrainData terrainData;
    private int alphamapWidth;
    private int alphamapHeight;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        collisionModule = ps.collision;

        if (terrain == null)
        {
            terrain = Terrain.activeTerrain;  // 기본 terrain을 사용
        }

        // Terrain 데이터와 알파맵 크기 캐시
        terrainData = terrain.terrainData;
        alphamapWidth = terrainData.alphamapWidth;
        alphamapHeight = terrainData.alphamapHeight;

        // 원본 알파 맵 값을 저장
        originalAlphaMap = terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);

        Debug.Log("Particle Collision Handler Initialized.");
    }

    void OnParticleCollision(GameObject other)
    {
        if (ps == null || other == null) return;

        // "Ground" 태그가 있는 객체와만 충돌 처리
        if (other.CompareTag("Ground"))
        {
            List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
            ps.GetCollisionEvents(other, collisionEvents);

            if (collisionEvents.Count == 0)
            {
                Debug.Log("충돌이 감지되지 않았습니다.");
            }

            // 충돌 이벤트를 처리
            foreach (var collisionEvent in collisionEvents)
            {
                Vector3 collisionPoint = collisionEvent.intersection;
                ModifyTerrainAlphaMap(collisionPoint);
            }
        }
    }

    void ModifyTerrainAlphaMap(Vector3 collisionPoint)
    {
        Vector3 terrainPos = terrain.transform.position;

        // 충돌 지점을 Terrain 좌표계로 변환
        float normalizedX = (collisionPoint.x - terrainPos.x) / terrainData.size.x;
        float normalizedZ = (collisionPoint.z - terrainPos.z) / terrainData.size.z;

        // x와 z를 교환하여 알파 맵 좌표 계산
        int xBase = Mathf.FloorToInt(normalizedZ * alphamapWidth);
        int zBase = Mathf.FloorToInt(normalizedX * alphamapHeight);

        // 클램프하여 알파맵 범위를 벗어나지 않도록 제한
        xBase = Mathf.Clamp(xBase, 0, alphamapWidth - 1);
        zBase = Mathf.Clamp(zBase, 0, alphamapHeight - 1);

        // 알파 맵 가져오기
        float[,,] alphaMap = terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);

        // 반경 내에서만 수정: radiusSquared를 float로 계산
        float radiusSquared = radius * radius;  // float로 계산

        for (int x = -Mathf.FloorToInt(radius); x <= Mathf.FloorToInt(radius); x++) // `int`로 반경을 계산
        {
            for (int z = -Mathf.FloorToInt(radius); z <= Mathf.FloorToInt(radius); z++) // `int`로 반경을 계산
            {
                // 반경 내에 있는지 체크
                if (x * x + z * z <= radiusSquared)
                {
                    int xOffset = Mathf.Clamp(xBase + x, 0, alphamapWidth - 1);
                    int zOffset = Mathf.Clamp(zBase + z, 0, alphamapHeight - 1);

                    // 알파 맵의 첫 번째 레이어는 0으로, 두 번째 레이어는 1로 설정
                    alphaMap[xOffset, zOffset, 0] = 0f;
                    alphaMap[xOffset, zOffset, 1] = 1f;
                }
            }
        }

        // 수정된 알파 맵을 적용
        terrainData.SetAlphamaps(0, 0, alphaMap);
    }




    // 게임 종료 시 알파 맵을 원래 상태로 복구
    void OnApplicationQuit()
    {
        if (terrain != null)
        {
            terrainData.SetAlphamaps(0, 0, originalAlphaMap);
            Debug.Log("알파 맵이 원래 상태로 복구되었습니다.");
        }
    }
}
