using UnityEngine;
using UnityEngine.EventSystems;
using System;
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f; // �⺻ �̵� �ӵ�
    public float rotationSpeed = 5f; // ��ü ȸ�� �ӵ�
    public Action onGameOver;    // �ܺο��� ������ �� �ִ� �ݹ� �׼� ���ӸŴ��� ���ӿ����� �˸��� �뵵
    public Action onHit;    // �ܺο��� ������ �� �ִ� �ݹ� �׼� ���ӸŴ��� ���ӿ����� �˸��� �뵵
    public float healTime = 5;//ȸ������ �ɸ��� �ð�
    public int hitCnt = 0; // ���� ���� ī��Ʈ ����

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity; // �ӵ� ����
    private Camera mainCamera; // ī�޶�
    private int gameOverCnt = 5; // 5�� ������ ���̸� ����ϵ��� ���� ����
    private float curHealTime = 0;//���� ȸ���Ǵ� �ð� ���



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
        if (!animator.GetBool("UnderAttack"))
        {
            MovePlayer();
        }
        if (animator.GetBool("UnderAttack"))
        {
            // CharacterController�� �̵�
            if (controller.isGrounded)
            {
                controller.Move(velocity * Time.deltaTime * 20f);
            }
            transform.rotation = Quaternion.LookRotation(new Vector3(velocity.x * -1f, 0f ,velocity.z * -1f));
        }
        // ���� �߻� ó��
        ShootChoco();

        // �ִϸ����� �Ķ���� ������Ʈ
        UpdateAnimator();
        //Debug.Log(velocity);
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
            bool randomValue = UnityEngine.Random.value > 0.5f;  // 0.5���� ū ���̸� true, �׷��� ������ false
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
            // "MallowBot"���� �浹 ó��
            if (hit.collider.CompareTag("MallowBot"))
            {
                BotController CheckedDummy = hit.transform.GetComponent<BotController>();
                if (CheckedDummy.isDown || animator.GetBool("UnderAttack") == true) return; // �ٿ� ���°ų� ���ݹ��� ���¸� ó�� ����


                if (animator)
                {
                    // MallowBot�� �ݴ� �������� �̵��ϴ� ����
                    Vector3 direction = (transform.position - hit.transform.position).normalized;

                    velocity = direction;
                    if(Mathf.Abs(velocity.y) >= 0.5) // y ���� 0.5 �����̸� x,y ���� �������� �Ͽ� ������ �� �з�������
                    {
                        velocity = new Vector3(UnityEngine.Random.value-0.5f, 0, UnityEngine.Random.value-0.5f).normalized;
                        
                    }

                    Debug.Log("�浹 ����" + velocity);
                    animator.SetBool("UnderAttack", true);

                    bool randomValue = UnityEngine.Random.value > 0.5f;
                    animator.SetBool("IsLeft", randomValue);
                    if (hitCnt < gameOverCnt)
                    {
                        hitCnt++;
                        onHit?.Invoke(); // �ܺ� �ݹ�
                    }
                    if (hitCnt >= gameOverCnt)
                    {
                        onGameOver?.Invoke();
                    }

                    // 0.5�� �� StopDrop ȣ��
                    Invoke("StopDrop", 0.5f);
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
