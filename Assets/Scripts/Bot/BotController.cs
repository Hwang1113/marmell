using UnityEngine;
using System.Collections;
using System;

public class BotController : MonoBehaviour
{
    public float moveSpeed = 5f; // 기본 이동 속도
    public float sprintSpeed = 6f; // 달리기 속도
    public float jumpHeight = 10f; // 점프 높이
    public float lowJumpHeight = 5f; // 점프 높이
    public float followDistance = 2.5f; // 플레이어와의 추적 거리
    public float jumpDistance = 10f; // 점프 거리 (플레이어가 가까워졌을 때 점프)
    public float groundCheckDistance = 0.3f; // 바닥 체크 거리
    public float minHeight = 0f;  // 최소 height 값
    public float maxHeight = 1.5f;  // 최대 height 값
    public float pitchTolerance = 15f; // 기울어짐 허용 범위 (15도)    
    public float scaleDuration = 2f;    // 코루틴에서 사용할 시간 (스케일이 완전히 0이 될 때까지의 시간)
    public bool isGrounded; // 땅에 있는지 확인
    public bool hasJumped = false; // 점프가 시작되었는지 확인하는 변수
    public bool isDown = false; // "Down" 상태 확인 변수
    public MaterialSwitcherController M_SwitchControl;
    public Action onDummyComplete;    // 외부에서 설정할 수 있는 콜백 액션
    public Action onCombo;    // 외부에서 설정할 수 있는 콜백 액션
    public Vector3 velocity; // 속도 벡터

    private CharacterController controller;
    private Animator animator;
    private Transform playerTransform; // 플레이어의 Transform
    private Vector3 jumpDirection; // 점프 시작 시의 이동 방향을 저장하는 변수
    private int jumpBoostpower = 2; // 점프했을때 곱하는 가속
    private int colCnt = 0; // 초코랑 충돌한 횟수 4번 충돌하면 초코멜로
    private int maxColCnt = 4;
    private float jumpTime = 0f;
    private ObjectActivator objectActivator; // ObjectActivator 컴포넌트



    void Start()
    {
        // 필요한 컴포넌트 가져오기
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        M_SwitchControl = GetComponent<MaterialSwitcherController>();

        // 플레이어의 Transform을 찾기
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // ObjectActivator 컴포넌트 찾기
        objectActivator = GetComponentInChildren<ObjectActivator>();
        // ObjectActivator의 스케일링 시작 후 실행할 작업을 설정
        objectActivator.onScaleStart = OnScaleStart;
        // ObjectActivator의 스케일링이 완료된 후 실행할 작업을 설정
        objectActivator.onScaleComplete = OnScaleComplete;

    }

    void Update()
    {
        // 다운 상태가 아닐 때만 이동 처리
        if (!isDown)
        {
            MoveBot();
        }

        // 컨트롤러 높이 조정
        AdjustHeightBasedOnTilt();

        // 애니메이터 파라미터 업데이트
        UpdateAnimator();

        // 점프 처리
        JumpCheck();

        if (!isDown)
        {
            // 실제 이동: CharacterController.Move()로 이동
            controller.Move(velocity * Time.deltaTime);
        }

        // colCnt가 4 이상일 경우 ObjectActivator 활성화
        if (colCnt >= maxColCnt && objectActivator != null)
        {
            objectActivator.ActivateObject(); // 오브젝트 활성화
        }
    }

    void MoveBot()
    {
        if (playerTransform == null) return;

        // 플레이어와의 거리 계산 (xz 평면만 사용)
        float distanceToPlayerSquared = (new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(playerTransform.position.x, 0, playerTransform.position.z)).sqrMagnitude;
        float jumpDistanceSquared = jumpDistance * jumpDistance;

        // 플레이어가 일정 거리 이내에 있으면 점프
        if (distanceToPlayerSquared <= jumpDistanceSquared && isGrounded && !hasJumped && playerTransform.position.y >= transform.position.y)
        {
            Jump();
        }

        // 플레이어를 향해 이동 (카메라와는 관계 없이, 플레이어의 방향으로만)
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // 이동 방향으로 부드럽게 회전
        if (directionToPlayer.magnitude >= 0.1f)
        {
            float rotationSpeed = 5f;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            Vector3 targetEulerAngles = targetRotation.eulerAngles;

            targetEulerAngles.x = Mathf.Clamp(targetEulerAngles.x, -pitchTolerance, pitchTolerance);
            targetRotation = Quaternion.Euler(targetEulerAngles);

            if (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
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
            float random = UnityEngine.Random.Range(lowJumpHeight, jumpHeight);

            velocity.y = random;
            jumpDirection = (playerTransform.position - transform.position).normalized;  // 점프 시 이동 방향 저장
            animator.SetTrigger("Jump");  // 점프 애니메이션 트리거

            hasJumped = true;  // 점프 시작됨을 표시
        }
    }

    void JumpCheck()
    {
        // CharacterController의 isGrounded를 사용하여 바닥에 닿았는지 확인
        isGrounded = controller.isGrounded;
        if (hasJumped)
        {
            jumpTime += Time.deltaTime;
        }
        // 바닥에 닿으면 Y 속도를 초기화하고 점프 상태 종료
        if (isGrounded && jumpTime > 0.5f)
        {
            velocity.y = -2f;  // 바닥에 닿으면 아래로 밀어넣음
            hasJumped = false;  // 점프 완료

            // 점프 후 원래 플레이어 방향으로 이동하도록 `jumpDirection` 리셋
            jumpDirection = Vector3.zero;
            jumpTime = 0;
        }
    }

    void UpdateAnimator()
    {
        // 이동 상태 업데이트: Speed 파라미터 값 설정
        float speed = velocity.magnitude;
        // Speed가 0인 경우 애니메이터에서 Speed를 0으로 설정
        // isDown이 true일 경우 Speed는 0으로 설정하여 달리기 애니메이션이 나오지 않도록 처리
        if (isDown)
        {
            animator.SetFloat("Speed", 0);
        }
        else
        {
            // Speed 값은 항상 갱신 (velocity 값에 따라 업데이트)
            animator.SetFloat("Speed", speed);
        }

        // 점프 상태 업데이트: Jump 파라미터로 점프 상태 확인
        if (hasJumped)
        {
            animator.SetBool("IsJumping", true);
        }
        else
        {
            animator.SetBool("IsJumping", false);
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
        if (!isDown)
        {
            // 오브젝트의 기울기를 구하기 위해 transform.up 벡터를 사용
            Vector3 up = transform.up;  // 캐릭터의 위 방향
                                        // 월드 좌표계에서 기울기 각도 계산 (transform.up과 Vector3.up의 각도 차이)
            float angle = Vector3.Angle(up, Vector3.up);  // XZ 평면에서 기울어진 각도 계산

            // 기울기(angle)를 바탕으로 height 값을 계산 (0에서 1.5 사이)
            // 기울기가 0일 때는 minHeight, 90일 때는 maxHeight
            float height = Mathf.Lerp(maxHeight, minHeight, Mathf.InverseLerp(0f, 90f, angle));

            Vector3 newCenter = controller.center;
            newCenter.y = 0.75f;
            controller.center = newCenter;

            // height 값을 캐릭터 컨트롤러에 적용
            controller.height = height;
        }
        if (isDown)
        {
            Vector3 newCenter = controller.center;
            newCenter.y = 0.25f;
            controller.center = newCenter;
        }
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
        onCombo?.Invoke(); //콤보 콜백
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
    // 스케일링 시작 후 호출될 콜백 메서드
    private void OnScaleStart()
    {
        Debug.Log("초코볼화 시작");
        // 절반 스케일 코루틴 실행
        StartCoroutine(ScaleToHalf());
        // 캐릭터 컨트롤러 비활성화

    }
    private IEnumerator ScaleToHalf()
    {
        Vector3 initialScale = transform.localScale; // 초기 스케일 저장
        Vector3 targetScale = Vector3.one / 2f;     // 목표 스케일 (Vector3.one / 2)

        float elapsedTime = 0f; // 경과 시간

        while (elapsedTime < scaleDuration)
        {
            // 경과 시간에 비례하여 스케일 계산
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / scaleDuration);

            elapsedTime += Time.deltaTime; // 시간 증가
            yield return null; // 한 프레임 대기
        }

        // 마지막에는 정확히 목표 스케일로 설정
        transform.localScale = targetScale;
    }

    // 스케일링이 끝난 후 호출될 콜백 메서드
    private void OnScaleComplete()
    {
        Debug.Log("초코볼화 완료 후 최적화 시작");
        DisableAnimatorAndSkinnedMeshRenderers();
        onDummyComplete?.Invoke(); //더미화 완료 콜백
        controller.enabled = false;

    }

    // 애니메이터와 자식 SkinnedMeshRenderer들을 비활성화하는 함수
    // 애니메이터 비활성화와 스킨 렌더러, 스케일 줄이기 (코루틴 사용)
    private void DisableAnimatorAndSkinnedMeshRenderers()
    {
        // 애니메이터 비활성화
        animator.enabled = false;

        // 스킨 렌더러들 가져옴
        SkinnedMeshRenderer[] skinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var skinnedMesh in skinnedMeshes)
        {
            // 스킨 렌더러 비활성화
            skinnedMesh.enabled = false;
        }
    }
}
