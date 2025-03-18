using UnityEngine;
using System.Collections;
using System;

public class BotController : MonoBehaviour
{
    public float moveSpeed = 5f; // �⺻ �̵� �ӵ�
    public float sprintSpeed = 6f; // �޸��� �ӵ�
    public float jumpHeight = 10f; // ���� ����
    public float lowJumpHeight = 5f; // ���� ����
    public float followDistance = 2.5f; // �÷��̾���� ���� �Ÿ�
    public float jumpDistance = 10f; // ���� �Ÿ� (�÷��̾ ��������� �� ����)
    public float groundCheckDistance = 0.3f; // �ٴ� üũ �Ÿ�
    public float minHeight = 0f;  // �ּ� height ��
    public float maxHeight = 1.5f;  // �ִ� height ��
    public float pitchTolerance = 15f; // ������ ��� ���� (15��)    
    public float scaleDuration = 2f;    // �ڷ�ƾ���� ����� �ð� (�������� ������ 0�� �� �������� �ð�)
    public bool isGrounded; // ���� �ִ��� Ȯ��
    public bool hasJumped = false; // ������ ���۵Ǿ����� Ȯ���ϴ� ����
    public bool isDown = false; // "Down" ���� Ȯ�� ����
    public MaterialSwitcherController M_SwitchControl;
    public Action onDummyComplete;    // �ܺο��� ������ �� �ִ� �ݹ� �׼�
    public Action onCombo;    // �ܺο��� ������ �� �ִ� �ݹ� �׼�
    public Vector3 velocity; // �ӵ� ����

    private CharacterController controller;
    private Animator animator;
    private Transform playerTransform; // �÷��̾��� Transform
    private Vector3 jumpDirection; // ���� ���� ���� �̵� ������ �����ϴ� ����
    private int jumpBoostpower = 2; // ���������� ���ϴ� ����
    private int colCnt = 0; // ���ڶ� �浹�� Ƚ�� 4�� �浹�ϸ� ���ڸ��
    private int maxColCnt = 4;
    private float jumpTime = 0f;
    private ObjectActivator objectActivator; // ObjectActivator ������Ʈ



    void Start()
    {
        // �ʿ��� ������Ʈ ��������
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        M_SwitchControl = GetComponent<MaterialSwitcherController>();

        // �÷��̾��� Transform�� ã��
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // ObjectActivator ������Ʈ ã��
        objectActivator = GetComponentInChildren<ObjectActivator>();
        // ObjectActivator�� �����ϸ� ���� �� ������ �۾��� ����
        objectActivator.onScaleStart = OnScaleStart;
        // ObjectActivator�� �����ϸ��� �Ϸ�� �� ������ �۾��� ����
        objectActivator.onScaleComplete = OnScaleComplete;

    }

    void Update()
    {
        // �ٿ� ���°� �ƴ� ���� �̵� ó��
        if (!isDown)
        {
            MoveBot();
        }

        // ��Ʈ�ѷ� ���� ����
        AdjustHeightBasedOnTilt();

        // �ִϸ����� �Ķ���� ������Ʈ
        UpdateAnimator();

        // ���� ó��
        JumpCheck();

        if (!isDown)
        {
            // ���� �̵�: CharacterController.Move()�� �̵�
            controller.Move(velocity * Time.deltaTime);
        }

        // colCnt�� 4 �̻��� ��� ObjectActivator Ȱ��ȭ
        if (colCnt >= maxColCnt && objectActivator != null)
        {
            objectActivator.ActivateObject(); // ������Ʈ Ȱ��ȭ
        }
    }

    void MoveBot()
    {
        if (playerTransform == null) return;

        // �÷��̾���� �Ÿ� ��� (xz ��鸸 ���)
        float distanceToPlayerSquared = (new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(playerTransform.position.x, 0, playerTransform.position.z)).sqrMagnitude;
        float jumpDistanceSquared = jumpDistance * jumpDistance;

        // �÷��̾ ���� �Ÿ� �̳��� ������ ����
        if (distanceToPlayerSquared <= jumpDistanceSquared && isGrounded && !hasJumped && playerTransform.position.y >= transform.position.y)
        {
            Jump();
        }

        // �÷��̾ ���� �̵� (ī�޶�ʹ� ���� ����, �÷��̾��� �������θ�)
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // �̵� �������� �ε巴�� ȸ��
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

        // �׻� �޸��� (�ȱ� ����)
        float currentSpeed = sprintSpeed;

        // ���� �߿��� �̵� ���� �ӵ� 2�� ����
        if (hasJumped)
        {
            velocity = new Vector3(jumpDirection.x * currentSpeed * jumpBoostpower, velocity.y, jumpDirection.z * currentSpeed * jumpBoostpower);
        }
        else
        {
            // �������� �ʾ��� ���� �÷��̾� �������� �̵�
            velocity = new Vector3(directionToPlayer.x * currentSpeed, velocity.y, directionToPlayer.z * currentSpeed);
        }

        // ���� �� �߷� �ݿ�
        if (!isGrounded)
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;  // �߷� ����
        }
        else
        {
            // �ٴڿ� ������ Y �ӵ��� -2�� �����Ͽ� �ε巯�� ������ ����
            velocity.y = Mathf.Max(velocity.y, -2f);  // �ʹ� �������� �ʰ� ����
        }
    }

    void Jump()
    {
        // ���� ����: jumpHeight�� �̿��� ���� �ӵ� ��� 
        if (isGrounded)
        {
            float random = UnityEngine.Random.Range(lowJumpHeight, jumpHeight);

            velocity.y = random;
            jumpDirection = (playerTransform.position - transform.position).normalized;  // ���� �� �̵� ���� ����
            animator.SetTrigger("Jump");  // ���� �ִϸ��̼� Ʈ����

            hasJumped = true;  // ���� ���۵��� ǥ��
        }
    }

    void JumpCheck()
    {
        // CharacterController�� isGrounded�� ����Ͽ� �ٴڿ� ��Ҵ��� Ȯ��
        isGrounded = controller.isGrounded;
        if (hasJumped)
        {
            jumpTime += Time.deltaTime;
        }
        // �ٴڿ� ������ Y �ӵ��� �ʱ�ȭ�ϰ� ���� ���� ����
        if (isGrounded && jumpTime > 0.5f)
        {
            velocity.y = -2f;  // �ٴڿ� ������ �Ʒ��� �о����
            hasJumped = false;  // ���� �Ϸ�

            // ���� �� ���� �÷��̾� �������� �̵��ϵ��� `jumpDirection` ����
            jumpDirection = Vector3.zero;
            jumpTime = 0;
        }
    }

    void UpdateAnimator()
    {
        // �̵� ���� ������Ʈ: Speed �Ķ���� �� ����
        float speed = velocity.magnitude;
        // Speed�� 0�� ��� �ִϸ����Ϳ��� Speed�� 0���� ����
        // isDown�� true�� ��� Speed�� 0���� �����Ͽ� �޸��� �ִϸ��̼��� ������ �ʵ��� ó��
        if (isDown)
        {
            animator.SetFloat("Speed", 0);
        }
        else
        {
            // Speed ���� �׻� ���� (velocity ���� ���� ������Ʈ)
            animator.SetFloat("Speed", speed);
        }

        // ���� ���� ������Ʈ: Jump �Ķ���ͷ� ���� ���� Ȯ��
        if (hasJumped)
        {
            animator.SetBool("IsJumping", true);
        }
        else
        {
            animator.SetBool("IsJumping", false);
        }

        // "Down" ���� üũ
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
            // ������Ʈ�� ���⸦ ���ϱ� ���� transform.up ���͸� ���
            Vector3 up = transform.up;  // ĳ������ �� ����
                                        // ���� ��ǥ�迡�� ���� ���� ��� (transform.up�� Vector3.up�� ���� ����)
            float angle = Vector3.Angle(up, Vector3.up);  // XZ ��鿡�� ������ ���� ���

            // ����(angle)�� �������� height ���� ��� (0���� 1.5 ����)
            // ���Ⱑ 0�� ���� minHeight, 90�� ���� maxHeight
            float height = Mathf.Lerp(maxHeight, minHeight, Mathf.InverseLerp(0f, 90f, angle));

            Vector3 newCenter = controller.center;
            newCenter.y = 0.75f;
            controller.center = newCenter;

            // height ���� ĳ���� ��Ʈ�ѷ��� ����
            controller.height = height;
        }
        if (isDown)
        {
            Vector3 newCenter = controller.center;
            newCenter.y = 0.25f;
            controller.center = newCenter;
        }
    }

    // �ٿ� ���� ���� Coroutine
    private IEnumerator RecoverFromDownState()
    {
        // 2�� ���
        yield return new WaitForSeconds(2f);

        // ���� ���� üũ
        if (colCnt >= 4)
        {
            // colCnt�� 4 �̻��� ��� �������� ����
            yield break;
        }

        // �ٿ� ���� ����
        isDown = false;
        animator.SetBool("IsDown", false);

        // ���� �� �̵� �����ϵ��� velocity �ʱ�ȭ (Y���� ����)
        velocity = new Vector3(0, velocity.y, 0);  // Y�� �ӵ��� �����ϸ� X, Z�� ����

        // �̵� �簳 (���� MoveBot() �Լ��� ��� ȣ��ǵ��� ����)
    }


    // �浹 �� ó���� ���� �Լ��� �и�
    private void HandleChocoCollision()
    {
        // ���� �浹 ī��Ʈ ���� �� ���͸��� ��ȯ
        ChocoColCntPlusCheck();

        // ������ �ٿ� ���·� ��ȯ
        animator.SetBool("IsDown", true);
        isDown = true;  // Down ���� ����

        // ���� ���� ����
        hasJumped = false;

        // X, Y, Z ��� 0���� �����Ͽ� �̵��� ������ ����
        velocity = Vector3.zero;

        // ĳ���� ��Ʈ�ѷ��� ���̸� �ּҷ� �ٲ�
        controller.height = minHeight;

        // colCnt�� 4 �̻��̸� ������ ���߰� ���� �ڷ�ƾ�� �������� ����
        if (colCnt >= 4)
        {
            return; // ������ ���� ����
        }
        onCombo?.Invoke(); //�޺� �ݹ�
        // colCnt�� 3 ������ ��� ���� ����
        StartCoroutine(RecoverFromDownState());
    }


    // �浹�� ��ü�� "Choco"���� Ȯ���ϰ� ó��
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Choco"))
        {
            HandleChocoCollision();
        }
    }

    // ��ƼŬ �浹 �� ó��
    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Choco"))
        {
            HandleChocoCollision();
        }
    }

    // ���� �浹 ī��Ʈ ���� �� ���͸��� ��ȯ ó��
    void ChocoColCntPlusCheck()
    {
        if (colCnt < maxColCnt)
        {
            colCnt++;  // ���� �浹 �߻��� ������ ī��Ʈ �߰�
            M_SwitchControl.SwitchNextMaterial();
        }
    }
    // �����ϸ� ���� �� ȣ��� �ݹ� �޼���
    private void OnScaleStart()
    {
        Debug.Log("���ں�ȭ ����");
        // ���� ������ �ڷ�ƾ ����
        StartCoroutine(ScaleToHalf());
        // ĳ���� ��Ʈ�ѷ� ��Ȱ��ȭ

    }
    private IEnumerator ScaleToHalf()
    {
        Vector3 initialScale = transform.localScale; // �ʱ� ������ ����
        Vector3 targetScale = Vector3.one / 2f;     // ��ǥ ������ (Vector3.one / 2)

        float elapsedTime = 0f; // ��� �ð�

        while (elapsedTime < scaleDuration)
        {
            // ��� �ð��� ����Ͽ� ������ ���
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / scaleDuration);

            elapsedTime += Time.deltaTime; // �ð� ����
            yield return null; // �� ������ ���
        }

        // ���������� ��Ȯ�� ��ǥ �����Ϸ� ����
        transform.localScale = targetScale;
    }

    // �����ϸ��� ���� �� ȣ��� �ݹ� �޼���
    private void OnScaleComplete()
    {
        Debug.Log("���ں�ȭ �Ϸ� �� ����ȭ ����");
        DisableAnimatorAndSkinnedMeshRenderers();
        onDummyComplete?.Invoke(); //����ȭ �Ϸ� �ݹ�
        controller.enabled = false;

    }

    // �ִϸ����Ϳ� �ڽ� SkinnedMeshRenderer���� ��Ȱ��ȭ�ϴ� �Լ�
    // �ִϸ����� ��Ȱ��ȭ�� ��Ų ������, ������ ���̱� (�ڷ�ƾ ���)
    private void DisableAnimatorAndSkinnedMeshRenderers()
    {
        // �ִϸ����� ��Ȱ��ȭ
        animator.enabled = false;

        // ��Ų �������� ������
        SkinnedMeshRenderer[] skinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var skinnedMesh in skinnedMeshes)
        {
            // ��Ų ������ ��Ȱ��ȭ
            skinnedMesh.enabled = false;
        }
    }
}
