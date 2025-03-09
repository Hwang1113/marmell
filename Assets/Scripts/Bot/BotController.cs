using UnityEngine;

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

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity; // �ӵ� ����
    private Transform playerTransform; // �÷��̾��� Transform
    private Vector3 jumpDirection; // ���� ���� ���� �̵� ������ �����ϴ� ����
    private int jumpBoostpower = 2; // ���������� ���ϴ� ����
    void Start()
    {
        // �ʿ��� ������Ʈ ��������
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // �÷��̾��� Transform�� ã��
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // ���� �̵� ó��
        MoveBot();

        // �ִϸ����� �Ķ���� ������Ʈ
        UpdateAnimator();

        // ���� ó��
        JumpCheck();

        // ���� �̵�: CharacterController.Move()�� �̵�
        controller.Move(velocity * Time.deltaTime);
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
        if (isGrounded && velocity.y < 1) // velocity.y <= 0 �̿����� ���� ���ܼ� �ٲ�
        {
            velocity.y = -2f;  // �ٴڿ� ������ �Ʒ��� �о����
            hasJumped = false;  // ���� �Ϸ�

            // ���� �� ���� �÷��̾� �������� �̵��ϵ��� `jumpDirection` ����
            jumpDirection = Vector3.zero;
        }
    }

    bool IsGroundedByRaycast()
    {
        // �ٴڿ� ��� �ִ��� Raycast�� Ȯ��
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
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
    }
}

