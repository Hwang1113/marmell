using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public string playerTag = "Player"; // 플레이어의 태그
    public Transform player; // 플레이어의 Transform
    public float rotationSpeed = 5f; // 회전 속도
    public float distance = 10f; // 플레이어와 카메라 간의 거리
    public float rotationDamping = 3f; // 회전 보정 속도

    public float minDistance = 5f; // 최소 거리
    public float maxDistance = 20f; // 최대 거리
    public float zoomSpeed = 2f; // 마우스 휠로 조정할 때의 속도

    public string collisionTag = "Ground"; // 충돌 감지할 태그

    public float maxVerticalAngle = 70f; // 수직 회전 제한 각도 (상향)
    public float minVerticalAngle = -70f; // 수직 회전 제한 각도 (하향)

    private float currentRotation = 0f; // 현재 회전 각도
    private float currentVerticalRotation = 0f; // 현재 수직 회전 각도
    private Vector3 velocity = Vector3.zero; // 이동 속도 (카메라 위치 보정용)

    void Start()
    {
        // "Player" 태그를 가진 게임 오브젝트를 찾고, 그 Transform을 player에 할당
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            player = playerObject.transform; // 플레이어 변수에 할당
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
            return; // 플레이어가 없으면 카메라 움직임을 처리하지 않음
        }

        // 마우스 이동에 따라 카메라 회전
        float horizontalInput = Input.GetAxis("Mouse X"); // 마우스 X 이동
        float verticalInput = Input.GetAxis("Mouse Y"); // 마우스 Y 이동

        // 수평 회전
        currentRotation += horizontalInput * rotationSpeed;

        // 수직 회전 (상하 방향 회전)
        currentVerticalRotation -= verticalInput * rotationSpeed; // Y축 입력을 반대로 처리

        // 수직 회전 제한 (카메라가 지나치게 위나 아래로 회전하지 않도록 제한)
        currentVerticalRotation = Mathf.Clamp(currentVerticalRotation, minVerticalAngle, maxVerticalAngle); // 회전 각도 제한

        // 마우스 휠로 카메라 거리 조정
        float scrollInput = Input.GetAxis("Mouse ScrollWheel"); // 마우스 휠 입력
        distance -= scrollInput * zoomSpeed; // 마우스 휠에 따라 거리 조정
        distance = Mathf.Clamp(distance, minDistance, maxDistance); // 최소, 최대 거리 제한

        // 카메라 회전 (수평 회전 및 수직 회전 적용)
        Quaternion rotation = Quaternion.Euler(currentVerticalRotation, currentRotation, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance); // 카메라와 플레이어 간의 상대적 위치

        // 카메라 위치 계산
        Vector3 newCameraPosition = player.position + offset;

        // 카메라 기준으로 아래로 레이 쏘기
        RaycastHit[] hits;
        Ray ray = new Ray(new Vector3(newCameraPosition.x, player.position.y + 20f, newCameraPosition.z), Vector3.down); // 플레이어 위치 기준 위에서 아래로 레이 쏘기

        hits = Physics.RaycastAll(ray, distance + 20f); // 거리 충돌 확인 (모든 충돌 지점 확인)

        // 충돌한 물체들 중에서 'Ground' 태그만 처리
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag(collisionTag))
            {
                // 카메라의 y 위치가 충돌 지점보다 낮아지지 않게 제한
                newCameraPosition.y = Mathf.Max(newCameraPosition.y, hit.point.y + 1f); // 0.5f로 오프셋을 조금 더 올리기
                break; // 'Ground' 태그를 찾으면 그 위치로 이동하고 나머지는 무시
            }
        }

        // 만약 원하는 태그와 충돌하지 않았다면, 기본적으로 계산된 카메라 위치를 사용
        if (newCameraPosition.y == player.position.y + offset.y)
        {
            newCameraPosition.y = Mathf.Max(newCameraPosition.y, player.position.y + offset.y);
        }

        // 디버그용 레이캐스트 선을 그리기 (카메라 기준 아래로)
        Debug.DrawRay(ray.origin, Vector3.down * (distance + 1f), Color.red); // 레이캐스트 선 (카메라 위치에서 아래로)

        // 카메라를 플레이어를 따라 다니도록 부드럽게 이동
        transform.position = Vector3.SmoothDamp(transform.position, newCameraPosition, ref velocity, rotationDamping * Time.deltaTime);

        // 카메라는 항상 플레이어를 바라보도록 회전
        transform.LookAt(player);
    }
}
