using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player; // �÷��̾��� Transform
    public float rotationSpeed = 5f; // ȸ�� �ӵ�
    public float distance = 5f; // �÷��̾�� ī�޶� ���� �Ÿ�
    public float height = 2f; // ī�޶��� ���� (�÷��̾��� ����)
    public float heightDamping = 2f; // ���� ���� �ӵ�
    public float rotationDamping = 3f; // ȸ�� ���� �ӵ�

    public float minDistance = 3f; // �ּ� �Ÿ�
    public float maxDistance = 10f; // �ִ� �Ÿ�
    public float zoomSpeed = 2f; // ���콺 �ٷ� ������ ���� �ӵ�

    private float currentRotation = 0f; // ���� ȸ�� ����
    private Vector3 velocity = Vector3.zero; // �̵� �ӵ� (ī�޶� ��ġ ������)

    void LateUpdate()
    {
        // ���콺 �̵��� ���� ī�޶� ȸ��
        float horizontalInput = Input.GetAxis("Mouse X"); // ���콺 X �̵�
        float verticalInput = Input.GetAxis("Mouse Y"); // ���콺 Y �̵�

        // ���콺 X�� ���� �÷��̾ �������� ȸ��
        currentRotation += horizontalInput * rotationSpeed;

        // ���콺 Y�� ���� ī�޶��� ���� ����
        float desiredHeight = player.position.y + height;
        float currentHeight = transform.position.y;

        // ī�޶��� ���̸� �ε巴�� ����
        currentHeight = Mathf.Lerp(currentHeight, desiredHeight, heightDamping * Time.deltaTime);

        // ���콺 �ٷ� ī�޶� �Ÿ� ����
        float scrollInput = Input.GetAxis("Mouse ScrollWheel"); // ���콺 �� �Է�
        distance -= scrollInput * zoomSpeed; // ���콺 �ٿ� ���� �Ÿ� ����
        distance = Mathf.Clamp(distance, minDistance, maxDistance); // �ּ�, �ִ� �Ÿ� ����

        // ī�޶� ��ġ ��� (�÷��̾� �������� ���� �Ÿ�)
        Quaternion rotation = Quaternion.Euler(0, currentRotation, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        // �÷��̾� ��ġ���� ī�޶��� ��ġ�� ����ϸ鼭 ���� ����
        Vector3 newCameraPosition = player.position + offset;
        newCameraPosition.y = currentHeight; // ���� ����

        // ī�޶� �÷��̾ ���� �ٴϵ��� �ε巴�� �̵�
        transform.position = Vector3.SmoothDamp(transform.position, newCameraPosition, ref velocity, rotationDamping * Time.deltaTime);

        // ī�޶� �׻� �÷��̾ �ٶ󺸵��� ȸ��
        transform.LookAt(player);
    }
}
