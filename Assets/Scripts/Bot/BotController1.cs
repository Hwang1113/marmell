using UnityEngine;

public class BotController1 : MonoBehaviour
{
    public float moveSpeed = 5f; // �⺻ �̵� �ӵ�
    public float sprintSpeed = 10f; // �޸��� �ӵ�
    public float jumpHeight = 2f; // ���� ����
    public float followDistance = 10f; // �÷��̾���� ���� �Ÿ�
    public float jumpDistance = 2f; // ���� �Ÿ� (�÷��̾ ��������� �� ����)
    public float groundCheckDistance = 0.2f; // �ٴ� üũ �Ÿ�

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity; // �ӵ� ����
    private Transform playerTransform; // �÷��̾��� Transform
    private bool isGrounded; // ���� �ִ��� Ȯ��

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
    }

    void MoveBot()
    {
        if (playerTransform == null) return;

        // �÷��̾���� �Ÿ� ���
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // �÷��̾ ���� �Ÿ� �̳��� ������ ����
        if (distanceToPlayer <= jumpDistance && isGrounded)
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

        // Shift Ű�� ������ ������ �̵� (�޸���)
        float currentSpeed = moveSpeed;

        // �÷��̾���� �Ÿ��� ���� �̻� �־����� �޸���
        if (distanceToPlayer > followDistance)
        {
            currentSpeed = sprintSpeed;
        }

        // �̵� ���⿡ �ӵ� ����
        velocity = directionToPlayer * currentSpeed;

        // CharacterController�� �̵�
        controller.Move(velocity * Time.deltaTime);
    }

    void JumpCheck()
    {
        // Raycast�� �ٴ� üũ�ϱ�
        isGrounded = IsGroundedByRaycast();

        // �ٴڿ� ������ Y �ӵ��� �ʱ�ȭ
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // ���� ������ �Ʒ��� �о����
        }
    }

    bool IsGroundedByRaycast()
    {
        // �ٴڿ� ��� �ִ��� Raycast�� Ȯ��
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }

    void Jump()
    {
        // ���� ����
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);

        // ���� �ִϸ��̼� Ʈ����
        animator.SetTrigger("Jump");
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
        else
        {
            animator.SetBool("IsJumping", true);
        }
    }
}
