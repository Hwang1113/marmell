using UnityEngine;
using System.Collections;

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
    public bool isDown = false; // "Down" 상태 확인 변수
    public MaterialSwitcherController M_SwitchControl;
    public float minHeight = 0f;  // 최소 height 값
    public float maxHeight = 1.5f;  // 최대 height 값

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity; // 속도 벡터
    private Transform playerTransform; // 플레이어의 Transform
    private Vector3 jumpDirection; // 점프 시작 시의 이동 방향을 저장하는 변수
    private int jumpBoostpower = 2; // 점프했을때 곱하는 가속
    private int colCnt = 0; // 초코랑 충돌한 횟수 4번 충돌하면 초코멜로
    private int maxColCnt = 4;

    void Start()
    {
        // 필요한 컴포넌트 가져오기
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        M_SwitchControl = GetComponent<MaterialSwitcherController>();
        // 플레이어의 Transform을 찾기
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // 다운 상태가 아닐 때만 이동 처리
        if (!isDown)
        {
            MoveBot();
            //컨트롤러 높이 조정
            AdjustHeightBasedOnTilt();
        }

        // 애니메이터 파라미터 업데이트
        UpdateAnimator();

        // 점프 처리
        JumpCheck();



        if (!isDown)
        {
            // 실제 이동: CharacterController.Move()로 이동
            controller.Move(velocity * Time.deltaTime);
        }
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
        if (isGrounded && velocity.y < 1) // velocity.y <= 0 이었으나 버그 생겨서 바꿈
        {
            velocity.y = -2f;  // 바닥에 닿으면 아래로 밀어넣음
            hasJumped = false;  // 점프 완료

            // 점프 후 원래 플레이어 방향으로 이동하도록 `jumpDirection` 리셋
            jumpDirection = Vector3.zero;
        }
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

        // "Down" 상태 체크
        if (isDown)
        {
            animator.SetBool("IsDown", true);
        }
        else
        {
            animator.SetBool("IsDown", false);
        }
    }

    void AdjustHeightBasedOnTilt()
    {
        // 오브젝트의 기울기를 구하기 위해 transform.up 벡터를 사용
        Vector3 up = transform.up;  // 캐릭터의 위 방향

        // 월드 좌표계에서 기울기 각도 계산 (transform.up과 Vector3.up의 각도 차이)
        float angle = Vector3.Angle(up, Vector3.up);  // XZ 평면에서 기울어진 각도 계산

        // 기울기(angle)를 바탕으로 height 값을 계산 (0에서 1.5 사이)
        // 기울기가 0일 때는 minHeight, 90일 때는 maxHeight
        float height = Mathf.Lerp(maxHeight, minHeight, Mathf.InverseLerp(0f, 90f, angle));

        if (isDown)
        {
            Vector3 newCenter = controller.center;
            newCenter.y = 0.25f;
            controller.center = newCenter;
        }

        // height 값을 캐릭터 컨트롤러에 적용
        controller.height = height;
    }

    // 다운 상태 복구 Coroutine
    private IEnumerator RecoverFromDownState()
    {
        // 2초 대기
        yield return new WaitForSeconds(2f);

        // 복구 조건 체크
        if (colCnt >= 4)
        {
            // colCnt가 4 이상일 경우 복구하지 않음
            yield break;
        }

        // 다운 상태 복구
        isDown = false;
        animator.SetBool("IsDown", false);

        // 복구 후 이동 가능하도록 velocity 초기화 (Y축은 유지)
        velocity = new Vector3(0, velocity.y, 0);  // Y축 속도는 유지하며 X, Z는 복구

        // 이동 재개 (이후 MoveBot() 함수가 계속 호출되도록 유지)
    }


    // 충돌 후 처리를 공통 함수로 분리
    private void HandleChocoCollision()
    {
        // 초코 충돌 카운트 증가 및 메터리얼 전환
        ChocoColCntPlusCheck();

        // 무조건 다운 상태로 전환
        animator.SetBool("IsDown", true);
        isDown = true;  // Down 상태 설정

        // 점프 상태 해제
        hasJumped = false;

        // X, Y, Z 모두 0으로 설정하여 이동을 완전히 멈춤
        velocity = Vector3.zero;

        // 캐릭터 컨트롤러의 높이를 최소로 바꿈
        controller.height = minHeight;

        // colCnt가 4 이상이면 복구를 멈추고 복구 코루틴을 시작하지 않음
        if (colCnt >= 4)
        {
            return; // 복구를 하지 않음
        }

        // colCnt가 3 이하일 경우 복구 시작
        StartCoroutine(RecoverFromDownState());
    }


    // 충돌한 객체가 "Choco"인지 확인하고 처리
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Choco"))
        {
            HandleChocoCollision();
        }
    }

    // 파티클 충돌 시 처리
    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Choco"))
        {
            HandleChocoCollision();
        }
    }

    // 초코 충돌 카운트 증가 및 메터리얼 전환 처리
    void ChocoColCntPlusCheck()
    {
        if (colCnt < maxColCnt)
        {
            colCnt++;  // 초코 충돌 발생할 때마다 카운트 추가
            M_SwitchControl.SwitchNextMaterial();
        }
    }
}
