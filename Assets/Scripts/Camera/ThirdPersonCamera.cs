using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public string playerTag = "Player"; // �÷��̾��� �±�
    public Transform player; // �÷��̾��� Transform
    public float rotationSpeed = 5f; // ȸ�� �ӵ�
    public float distance = 5f; // �÷��̾�� ī�޶� ���� �Ÿ�
    public float rotationDamping = 3f; // ȸ�� ���� �ӵ�

    public float minDistance = 5f; // �ּ� �Ÿ�
    public float maxDistance = 12f; // �ִ� �Ÿ�
    public float zoomSpeed = 2f; // ���콺 �ٷ� ������ ���� �ӵ�

    public string collisionTag = "Ground"; // �浹 ������ �±�

    public float maxVerticalAngle = 70f; // ���� ȸ�� ���� ���� (����)
    public float minVerticalAngle = -70f; // ���� ȸ�� ���� ���� (����)

    private float currentRotation = 0f; // ���� ȸ�� ����
    private float currentVerticalRotation = 0f; // ���� ���� ȸ�� ����
    private Vector3 velocity = Vector3.zero; // �̵� �ӵ� (ī�޶� ��ġ ������)

    void Start()
    {
        // "Player" �±׸� ���� ���� ������Ʈ�� ã��, �� Transform�� player�� �Ҵ�
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            player = playerObject.transform; // �÷��̾� ������ �Ҵ�
        }
        else
        {
            Debug.LogError("Player object with tag 'Player' not found!");
        }
    }

    void LateUpdate()
    {
        if (player == null)
        {
            Debug.LogWarning("Player not assigned!");
            return; // �÷��̾ ������ ī�޶� �������� ó������ ����
        }

        // ���콺 �̵��� ���� ī�޶� ȸ��
        float horizontalInput = Input.GetAxis("Mouse X"); // ���콺 X �̵�
        float verticalInput = Input.GetAxis("Mouse Y"); // ���콺 Y �̵�

        // ���� ȸ��
        currentRotation += horizontalInput * rotationSpeed;

        // ���� ȸ�� (���� ���� ȸ��)
        currentVerticalRotation -= verticalInput * rotationSpeed; // Y�� �Է��� �ݴ�� ó��

        // ���� ȸ�� ���� (ī�޶� ����ġ�� ���� �Ʒ��� ȸ������ �ʵ��� ����)
        currentVerticalRotation = Mathf.Clamp(currentVerticalRotation, minVerticalAngle, maxVerticalAngle); // ȸ�� ���� ����

        // ���콺 �ٷ� ī�޶� �Ÿ� ����
        float scrollInput = Input.GetAxis("Mouse ScrollWheel"); // ���콺 �� �Է�
        distance -= scrollInput * zoomSpeed; // ���콺 �ٿ� ���� �Ÿ� ����
        distance = Mathf.Clamp(distance, minDistance, maxDistance); // �ּ�, �ִ� �Ÿ� ����

        // ī�޶� ȸ�� (���� ȸ�� �� ���� ȸ�� ����)
        Quaternion rotation = Quaternion.Euler(currentVerticalRotation, currentRotation, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance); // ī�޶�� �÷��̾� ���� ����� ��ġ

        // ī�޶� ��ġ ���
        Vector3 newCameraPosition = player.position + offset;

        // ī�޶� �������� �Ʒ��� ���� ���
        RaycastHit hit;
        Ray ray = new Ray(new Vector3(newCameraPosition.x, player.position.y + 10f, newCameraPosition.z), Vector3.down); // �÷��̾� ��ġ ���� ������ �Ʒ��� ���� ���

        if (Physics.Raycast(ray, out hit, distance + 10f)) // �Ÿ� + 10f�� �����Ͽ�, �� ���� �������� �浹 Ȯ��
        {
            // �浹�� ��ü�� 'Ground' �±׸� ������ ������
            if (hit.collider.CompareTag(collisionTag))
            {
                // ī�޶��� y ��ġ�� �浹 �������� �������� �ʰ� ����
                newCameraPosition.y = Mathf.Max(newCameraPosition.y, hit.point.y + 0.5f); // 0.5f�� �������� ���� �� �ø���
            }
            else
            {
                // 'Ground'�� �ƴ� �ٸ� ��ü�� �浹�ϸ� ī�޶�� �� ��ġ�� �̵�
                newCameraPosition.y = hit.point.y; 
            }

            // ����׿� ����ĳ��Ʈ ���� �׸��� (ī�޶� ���� �Ʒ���)
            Debug.DrawRay(ray.origin, Vector3.down * (distance + 1f), Color.red); // ����ĳ��Ʈ �� (ī�޶� ��ġ���� �Ʒ���)
            Debug.DrawRay(ray.origin, hit.normal * 2f, Color.blue); // ���� ���� (�浹�� ǥ���� ���� ����)
        }

        // ī�޶� �÷��̾ ���� �ٴϵ��� �ε巴�� �̵�
        transform.position = Vector3.SmoothDamp(transform.position, newCameraPosition, ref velocity, rotationDamping * Time.deltaTime);

        // ī�޶�� �׻� �÷��̾ �ٶ󺸵��� ȸ��
        transform.LookAt(player);
    }
}
