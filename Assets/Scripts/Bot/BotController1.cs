using UnityEngine;

public class BotController1 : MonoBehaviour
{
    public float moveSpeed = 5f; // 기본 이동 속도
    public float sprintSpeed = 10f; // 달리기 속도
    public float jumpHeight = 2f; // 점프 높이
    public float followDistance = 10f; // 플레이어와의 추적 거리
    public float jumpDistance = 2f; // 점프 거리 (플레이어가 가까워졌을 때 점프)
    public float groundCheckDistance = 0.2f; // 바닥 체크 거리

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity; // 속도 벡터
    private Transform playerTransform; // 플레이어의 Transform
    private bool isGrounded; // 땅에 있는지 확인

    void Start()
    {
        // 필요한 컴포넌트 가져오기
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // 플레이어의 Transform을 찾기
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // 봇의 이동 처리
        MoveBot();

        // 애니메이터 파라미터 업데이트
        UpdateAnimator();

        // 점프 처리
        JumpCheck();
    }

    void MoveBot()
    {
        if (playerTransform == null) return;

        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // 플레이어가 일정 거리 이내에 있으면 점프
        if (distanceToPlayer <= jumpDistance && isGrounded)
        {
            Jump();
        }

        // 플레이어를 향해 이동 (카메라와는 관계 없이, 플레이어의 방향으로만)
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // 이동 방향으로 회전
        if (directionToPlayer.magnitude >= 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(directionToPlayer);
        }

        // Shift 키를 누르면 빠르게 이동 (달리기)
        float currentSpeed = moveSpeed;

        // 플레이어와의 거리가 일정 이상 멀어지면 달리기
        if (distanceToPlayer > followDistance)
        {
            currentSpeed = sprintSpeed;
        }

        // 이동 방향에 속도 적용
        velocity = directionToPlayer * currentSpeed;

        // CharacterController로 이동
        controller.Move(velocity * Time.deltaTime);
    }

    void JumpCheck()
    {
        // Raycast로 바닥 체크하기
        isGrounded = IsGroundedByRaycast();

        // 바닥에 닿으면 Y 속도를 초기화
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 땅에 닿으면 아래로 밀어넣음
        }
    }

    bool IsGroundedByRaycast()
    {
        // 바닥에 닿아 있는지 Raycast로 확인
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }

    void Jump()
    {
        // 점프 동작
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);

        // 점프 애니메이션 트리거
        animator.SetTrigger("Jump");
    }

    void UpdateAnimator()
    {
        // 이동 상태 업데이트: Speed 파라미터 값 설정
        float speed = new Vector3(velocity.x, 0, velocity.z).magnitude;
        animator.SetFloat("Speed", speed);

        // 점프 상태 업데이트: Jump 파라미터로 점프 상태 확인
        if (isGrounded)
        {
            animator.SetBool("IsJumping", false);
        }
        else
        {
            animator.SetBool("IsJumping", true);
        }
    }
}
