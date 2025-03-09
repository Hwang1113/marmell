using UnityEngine;

public class BotController : MonoBehaviour
{
    public float moveSpeed = 5f; // 기본 이동 속도
    public float sprintSpeed = 6f; // 달리기 속도
    public float jumpHeight = 3f; // 점프 높이
    public float followDistance = 2.5f; // 플레이어와의 추적 거리
    public float jumpDistance = 4f; // 점프 거리 (플레이어가 가까워졌을 때 점프)
    public float groundCheckDistance = 0.3f; // 바닥 체크 거리
    public bool isGrounded; // 땅에 있는지 확인
    public bool hasJumped = false; // 점프가 시작되었는지 확인하는 변수

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity; // 속도 벡터
    private Transform playerTransform; // 플레이어의 Transform
    private Vector3 jumpDirection; // 점프 시작 시의 이동 방향을 저장하는 변수
    private int jumpBoostpower = 2; // 점프했을때 곱하는 가속
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

        // 실제 이동: CharacterController.Move()로 이동
        controller.Move(velocity * Time.deltaTime);
    }

    void MoveBot()
    {
        if (playerTransform == null) return;

        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // 플레이어가 일정 거리 이내에 있으면 점프
        if (distanceToPlayer <= jumpDistance && isGrounded && !hasJumped)
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

        // 항상 달리기 (걷기 없이)
        float currentSpeed = sprintSpeed;

        // 점프 중에는 이동 방향 속도 2배 증가
        if (hasJumped)
        {
            velocity = new Vector3(jumpDirection.x * currentSpeed * jumpBoostpower, velocity.y, jumpDirection.z * currentSpeed * jumpBoostpower);
        }
        else
        {
            // 점프하지 않았을 때는 플레이어 방향으로 이동
            velocity = new Vector3(directionToPlayer.x * currentSpeed, velocity.y, directionToPlayer.z * currentSpeed);
        }

        // 점프 후 중력 반영
        if (!isGrounded)
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;  // 중력 적용
        }
        else
        {
            // 바닥에 닿으면 Y 속도를 -2로 설정하여 부드러운 착지를 유도
            velocity.y = Mathf.Max(velocity.y, -2f);  // 너무 낮아지지 않게 제한
        }
    }

    void Jump()
    {
        // 점프 동작: jumpHeight를 이용해 점프 속도 계산
        if (isGrounded)
        {
            velocity.y = jumpHeight;  // 점프 속도 설정
            jumpDirection = (playerTransform.position - transform.position).normalized;  // 점프 시 이동 방향 저장
            animator.SetTrigger("Jump");  // 점프 애니메이션 트리거

            hasJumped = true;  // 점프 시작됨을 표시
        }
    }

    void JumpCheck()
    {
        // CharacterController의 isGrounded를 사용하여 바닥에 닿았는지 확인
        isGrounded = controller.isGrounded;

        // 바닥에 닿으면 Y 속도를 초기화하고 점프 상태 종료
        if (isGrounded && velocity.y < 1) // velocity.y <= 0 이였으나 버그 생겨서 바꿈
        {
            velocity.y = -2f;  // 바닥에 닿으면 아래로 밀어넣음
            hasJumped = false;  // 점프 완료

            // 점프 후 원래 플레이어 방향으로 이동하도록 `jumpDirection` 리셋
            jumpDirection = Vector3.zero;
        }
    }

    bool IsGroundedByRaycast()
    {
        // 바닥에 닿아 있는지 Raycast로 확인
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
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
        else if (!isGrounded)
        {
            animator.SetBool("IsJumping", true);
        }
    }
}

