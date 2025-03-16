using UnityEngine;

public class SpaceBarrier : MonoBehaviour
{
    private bool isScaling = false;
    private float scaleDuration = 0.3f; // ũ�Ⱑ ���ϴ� �ð� (��)
    private float elapsedTime = 0f; // ��� �ð�
    private Collider sCollider = null;
    public Transform childtransform = null;
    public ParticleSystem chocoParticleSystem;
    public float maxSize = 4f;


    // Start is called once before the first frame update
    void Start()
    {
        childtransform = GetComponentsInChildren<Transform>()[1];
        childtransform.localScale = Vector3.zero; // �׻� �ʱⰪ�� 0���� ����
        sCollider = GetComponentInChildren<Collider>();
        sCollider.enabled = false;
        chocoParticleSystem = GetComponent<ParticleSystem>();
    } 

    // Update is called once per frame
    void Update()
    {
        // �����̽��ٰ� ������ ��
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isScaling = true;
            elapsedTime = 0f; // ��� �ð� �ʱ�ȭ
            sCollider.enabled = true;
            chocoParticleSystem.Play();
        }

        if (isScaling)
        {
            // scale�� 0���� maxSize�� ���������� ����
            elapsedTime += Time.deltaTime;
            float scaleValue = Mathf.Lerp(0f, maxSize, Mathf.Sqrt(elapsedTime / scaleDuration));

            // maxSize�� �����ϸ� �ٷ� 0���� ���ư����� ó��
            if (scaleValue >= maxSize)
            {
                scaleValue = maxSize; // scale�� maxSize�� �����ϸ� ���߰�,
                childtransform.localScale = Vector3.zero; // �ٷ� 0���� ����
                isScaling = false; // �����ϸ��� ���߰�
                sCollider.enabled = false;
            }
            else
            {
                // scale�� ����
                childtransform.localScale = Vector3.one * scaleValue;
            }
        }
    }
}
