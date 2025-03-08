using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // 기본 이동 속도
    public float sprintSpeed = 10f; // 달리기 속도

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity; // 속도 벡터
    private Camera mainCamera; // 카메라

    void Start()
    {
        // 필요한 컴포넌트 가져오기
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main; // 메인 카메라 가져오기
    }

    void Update()
    {
        // 이동 처리
        MovePlayer();

        // 애니메이터 파라미터 업데이트
        UpdateAnimator();
    }

    void MovePlayer()
    {
        // WASD 입력 받기
        float moveX = Input.GetAxis("Horizontal"); // A/D 키
        float moveZ = Input.GetAxis("Vertical");   // W/S 키

        // 카메라 기준으로 이동 방향 계산
        Vector3 forward = mainCamera.transform.forward; // 카메라의 앞 방향
        Vector3 right = mainCamera.transform.right;     // 카메라의 오른쪽 방향

        // 카메라의 Y축 회전만을 고려하여, 카메라는 수평 회전만 하도록 함
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // 카메라 기준으로 이동 방향 계산
        Vector3 moveDirection = (forward * moveZ + right * moveX).normalized;

        if (moveDirection.magnitude >= 0.1f) // 이동 방향이 있을 때만 회전
        {
            // 이동 방향으로 회전
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }

        // Shift 키를 누르면 빠르게 이동 (달리기)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        // 이동 방향에 속도 적용
        velocity = moveDirection * currentSpeed;

        // CharacterController로 이동
        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateAnimator()
    {
        // 이동 상태 업데이트: Speed 파라미터 값 설정
        float speed = new Vector3(velocity.x, 0, velocity.z).magnitude;
        animator.SetFloat("Speed", speed);
    }
}
