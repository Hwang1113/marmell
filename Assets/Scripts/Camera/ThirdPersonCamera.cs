using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public float rotationSpeed = 5f; // 회전 속도
    public float distance = 5f; // 플레이어와 카메라 간의 거리
    public float height = 2f; // 카메라의 높이 (플레이어의 위쪽)
    public float heightDamping = 2f; // 높이 보정 속도
    public float rotationDamping = 3f; // 회전 보정 속도

    public float minDistance = 3f; // 최소 거리
    public float maxDistance = 10f; // 최대 거리
    public float zoomSpeed = 2f; // 마우스 휠로 조정할 때의 속도

    private float currentRotation = 0f; // 현재 회전 각도
    private Vector3 velocity = Vector3.zero; // 이동 속도 (카메라 위치 보정용)

    void LateUpdate()
    {
        // 마우스 이동에 따라 카메라 회전
        float horizontalInput = Input.GetAxis("Mouse X"); // 마우스 X 이동
        float verticalInput = Input.GetAxis("Mouse Y"); // 마우스 Y 이동

        // 마우스 X에 따라 플레이어를 기준으로 회전
        currentRotation += horizontalInput * rotationSpeed;

        // 마우스 Y에 따라 카메라의 높이 조절
        float desiredHeight = player.position.y + height;
        float currentHeight = transform.position.y;

        // 카메라의 높이를 부드럽게 보정
        currentHeight = Mathf.Lerp(currentHeight, desiredHeight, heightDamping * Time.deltaTime);

        // 마우스 휠로 카메라 거리 조정
        float scrollInput = Input.GetAxis("Mouse ScrollWheel"); // 마우스 휠 입력
        distance -= scrollInput * zoomSpeed; // 마우스 휠에 따라 거리 조정
        distance = Mathf.Clamp(distance, minDistance, maxDistance); // 최소, 최대 거리 제한

        // 카메라 위치 계산 (플레이어 기준으로 일정 거리)
        Quaternion rotation = Quaternion.Euler(0, currentRotation, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        // 플레이어 위치에서 카메라의 위치를 계산하면서 높이 보정
        Vector3 newCameraPosition = player.position + offset;
        newCameraPosition.y = currentHeight; // 높이 보정

        // 카메라를 플레이어를 따라 다니도록 부드럽게 이동
        transform.position = Vector3.SmoothDamp(transform.position, newCameraPosition, ref velocity, rotationDamping * Time.deltaTime);

        // 카메라가 항상 플레이어를 바라보도록 회전
        transform.LookAt(player);
    }
}
