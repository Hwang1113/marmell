using UnityEngine;

public class SpaceBarrier : MonoBehaviour
{
    private bool isScaling = false;
    private float scaleDuration = 0.3f; // 크기가 변하는 시간 (초)
    private float elapsedTime = 0f; // 경과 시간
    private Collider sCollider = null;
    public Transform childtransform = null;
    public ParticleSystem chocoParticleSystem;
    public float maxSize = 4f;


    // Start is called once before the first frame update
    void Start()
    {
        childtransform = GetComponentsInChildren<Transform>()[1];
        childtransform.localScale = Vector3.zero; // 항상 초기값을 0으로 설정
        sCollider = GetComponentInChildren<Collider>();
        sCollider.enabled = false;
        chocoParticleSystem = GetComponent<ParticleSystem>();
    } 

    // Update is called once per frame
    void Update()
    {
        // 스페이스바가 눌렸을 때
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isScaling = true;
            elapsedTime = 0f; // 경과 시간 초기화
            sCollider.enabled = true;
            chocoParticleSystem.Play();
        }

        if (isScaling)
        {
            // scale을 0에서 maxSize로 점진적으로 증가
            elapsedTime += Time.deltaTime;
            float scaleValue = Mathf.Lerp(0f, maxSize, Mathf.Sqrt(elapsedTime / scaleDuration));

            // maxSize에 도달하면 바로 0으로 돌아가도록 처리
            if (scaleValue >= maxSize)
            {
                scaleValue = maxSize; // scale이 maxSize에 도달하면 멈추고,
                childtransform.localScale = Vector3.zero; // 바로 0으로 리셋
                isScaling = false; // 스케일링을 멈추고
                sCollider.enabled = false;
            }
            else
            {
                // scale을 적용
                childtransform.localScale = Vector3.one * scaleValue;
            }
        }
    }
}
