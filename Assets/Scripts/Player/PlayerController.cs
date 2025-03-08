using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // �⺻ �̵� �ӵ�
    public float sprintSpeed = 10f; // �޸��� �ӵ�

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity; // �ӵ� ����
    private Camera mainCamera; // ī�޶�

    void Start()
    {
        // �ʿ��� ������Ʈ ��������
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main; // ���� ī�޶� ��������
    }

    void Update()
    {
        // �̵� ó��
        MovePlayer();

        // �ִϸ����� �Ķ���� ������Ʈ
        UpdateAnimator();
    }

    void MovePlayer()
    {
        // WASD �Է� �ޱ�
        float moveX = Input.GetAxis("Horizontal"); // A/D Ű
        float moveZ = Input.GetAxis("Vertical");   // W/S Ű

        // ī�޶� �������� �̵� ���� ���
        Vector3 forward = mainCamera.transform.forward; // ī�޶��� �� ����
        Vector3 right = mainCamera.transform.right;     // ī�޶��� ������ ����

        // ī�޶��� Y�� ȸ������ ����Ͽ�, ī�޶�� ���� ȸ���� �ϵ��� ��
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // ī�޶� �������� �̵� ���� ���
        Vector3 moveDirection = (forward * moveZ + right * moveX).normalized;

        if (moveDirection.magnitude >= 0.1f) // �̵� ������ ���� ���� ȸ��
        {
            // �̵� �������� ȸ��
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }

        // Shift Ű�� ������ ������ �̵� (�޸���)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        // �̵� ���⿡ �ӵ� ����
        velocity = moveDirection * currentSpeed;

        // CharacterController�� �̵�
        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateAnimator()
    {
        // �̵� ���� ������Ʈ: Speed �Ķ���� �� ����
        float speed = new Vector3(velocity.x, 0, velocity.z).magnitude;
        animator.SetFloat("Speed", speed);
    }
}
