using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f; // �⺻ �̵� �ӵ�
    public float rotationSpeed = 5f; // ��ü ȸ�� �ӵ�

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

        ShootChoco();

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

        float currentSpeed = moveSpeed;

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

        // Ŭ������ �� ChocoShot ���·� ��ȯ
        if (Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư Ŭ��
        {
            animator.SetTrigger("ChocoShot"); // ChocoShot Ʈ���� ����
        }
    }

    void ShootChoco()
    {
        if (Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư Ŭ��
        {
            animator.SetBool("IsShooting", true);
            bool randomValue = Random.value > 0.5f;  // 0.5���� ū ���̸� true, �׷��� ������ false
            animator.SetBool("IsLeft", randomValue);  // �������� true �Ǵ� false�� ����
            Invoke("StopShooting", 1f);
        }
    }
    void StopShooting()
    {
        animator.SetBool("IsShooting", false);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // �浹�� ��ü�� "CharacterController"�� ��츸 ó��
        if (hit.collider.GetComponent<CharacterController>() != null)
        {
            Debug.Log("�浹�� ��ü�� CharacterController�Դϴ�.");

            // ���⼭ �浹 ó�� ������ �߰��մϴ�.
            if (hit.collider.CompareTag("MallowBot"))
            {
                if (animator != null)
                {
                    animator.SetBool("UnderAttack", true);

                    bool randomValue = Random.value > 0.5f;
                    animator.SetBool("IsLeft", randomValue);

                    // 1�� �� StopDrop ȣ��
                    Invoke("StopDrop", 1f);
                }
                else
                {
                    Debug.LogWarning("MallowBot animator not found.");
                }
            }
        }
    }


    void StopDrop()
    {
        animator.SetBool("UnderAttack", false);
    }
}
