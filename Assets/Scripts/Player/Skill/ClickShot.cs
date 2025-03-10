using UnityEngine;

public class ClickShot : MonoBehaviour
{
    private ParticleSystem particleSystem;  // 파티클 시스템을 참조할 변수

    void Start()
    {
        // 현재 오브젝트에 있는 ParticleSystem 컴포넌트를 자동으로 찾음
        particleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // 마우스 왼쪽 버튼 클릭 시 파티클 실행
        if (Input.GetMouseButtonDown(0))  // 0은 좌클릭을 의미
        {
            if (particleSystem != null)
            {
                particleSystem.Play();  // 파티클 실행
            }
        }
    }
}
