using UnityEngine;
using System.Collections;

public class BotController : MonoBehaviour
{
    public float moveSpeed = 5f; // �⺻ �̵� �ӵ�
    public float sprintSpeed = 6f; // �޸��� �ӵ�
    public float jumpHeight = 3f; // ���� ����
    public float followDistance = 2.5f; // �÷��̾���� ���� �Ÿ�
    public float jumpDistance = 4f; // ���� �Ÿ� (�÷��̾ ��������� �� ����)
    public float groundCheckDistance = 0.3f; // �ٴ� üũ �Ÿ�
    public bool isGrounded; // ���� �ִ��� Ȯ��
    public bool hasJumped = false; // ������ ���۵Ǿ����� Ȯ���ϴ� ����
    public bool isDown = false; // "Down" ���� Ȯ�� ����
    public MaterialSwitcherController M_SwitchControl;
    public float minHeight = 0f;  // �ּ� height ��
    public float maxHeight = 1.5f;  // �ִ� height ��

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity; // �ӵ� ����
    private Transform playerTransform; // �÷��̾��� Transform
    private Vector3 jumpDirection; // ���� ���� ���� �̵� ������ �����ϴ� ����
    private int jumpBoostpower = 2; // ���������� ���ϴ� ����
    private int colCnt = 0; // ���ڶ� �浹�� Ƚ�� 4�� �浹�ϸ� ���ڸ��
    private int maxColCnt = 4;

    void Start()
    {
        // �ʿ��� ������Ʈ ��������
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        M_SwitchControl = GetComponent<MaterialSwitcherController>();
        // �÷��̾��� Transform�� ã��
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // �ٿ� ���°� �ƴ� ���� �̵� ó��
        if (!isDown)
        {
            MoveBot();
            //��Ʈ�ѷ� ���� ����
            AdjustHeightBasedOnTilt();
        }

        // �ִϸ����� �Ķ���� ������Ʈ
        UpdateAnimator();

        // ���� ó��
        JumpCheck();



        if (!isDown)
        {
            // ���� �̵�: CharacterController.Move()�� �̵�
            controller.Move(velocity * Time.deltaTime);
        }
    }

    void MoveBot()
    {
        if (playerTransform == null) return;

        // �÷��̾���� �Ÿ� ���
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // �÷��̾ ���� �Ÿ� �̳��� ������ ����
        if (distanceToPlayer <= jumpDistance && isGrounded && !hasJumped)
        {
            Jump();
        }

        // �÷��̾ ���� �̵� (ī�޶�ʹ� ���� ����, �÷��̾��� �������θ�)
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // �̵� �������� ȸ��
        if (directionToPlayer.magnitude >= 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(directionToPlayer);
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
            velocity.y = jumpHeight;  // ���� �ӵ� ����
            jumpDirection = (playerTransform.position - transform.position).normalized;  // ���� �� �̵� ���� ����
            animator.SetTrigger("Jump");  // ���� �ִϸ��̼� Ʈ����

            hasJumped = true;  // ���� ���۵��� ǥ��
        }
    }

    void JumpCheck()
    {
        // CharacterController�� isGrounded�� ����Ͽ� �ٴڿ� ��Ҵ��� Ȯ��
        isGrounded = controller.isGrounded;

        // �ٴڿ� ������ Y �ӵ��� �ʱ�ȭ�ϰ� ���� ���� ����
        if (isGrounded && velocity.y < 1) // velocity.y <= 0 �̾����� ���� ���ܼ� �ٲ�
        {
            velocity.y = -2f;  // �ٴڿ� ������ �Ʒ��� �о����
            hasJumped = false;  // ���� �Ϸ�

            // ���� �� ���� �÷��̾� �������� �̵��ϵ��� `jumpDirection` ����
            jumpDirection = Vector3.zero;
        }
    }

    void UpdateAnimator()
    {
        // �̵� ���� ������Ʈ: Speed �Ķ���� �� ����
        float speed = new Vector3(velocity.x, 0, velocity.z).magnitude;
        animator.SetFloat("Speed", speed);

        // ���� ���� ������Ʈ: Jump �Ķ���ͷ� ���� ���� Ȯ��
        if (isGrounded)
        {
            animator.SetBool("IsJumping", false);
        }
        else if (!isGrounded)
        {
            animator.SetBool("IsJumping", true);
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
        // ������Ʈ�� ���⸦ ���ϱ� ���� transform.up ���͸� ���
        Vector3 up = transform.up;  // ĳ������ �� ����

        // ���� ��ǥ�迡�� ���� ���� ��� (transform.up�� Vector3.up�� ���� ����)
        float angle = Vector3.Angle(up, Vector3.up);  // XZ ��鿡�� ������ ���� ���

        // ����(angle)�� �������� height ���� ��� (0���� 1.5 ����)
        // ���Ⱑ 0�� ���� minHeight, 90�� ���� maxHeight
        float height = Mathf.Lerp(maxHeight, minHeight, Mathf.InverseLerp(0f, 90f, angle));

        if (isDown)
        {
            Vector3 newCenter = controller.center;
            newCenter.y = 0.25f;
            controller.center = newCenter;
        }

        // height ���� ĳ���� ��Ʈ�ѷ��� ����
        controller.height = height;
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
}
