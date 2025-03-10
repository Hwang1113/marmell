using UnityEngine;

public class ClickShot : MonoBehaviour
{
    private ParticleSystem particleSystem;  // ��ƼŬ �ý����� ������ ����

    void Start()
    {
        // ���� ������Ʈ�� �ִ� ParticleSystem ������Ʈ�� �ڵ����� ã��
        particleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // ���콺 ���� ��ư Ŭ�� �� ��ƼŬ ����
        if (Input.GetMouseButtonDown(0))  // 0�� ��Ŭ���� �ǹ�
        {
            if (particleSystem != null)
            {
                particleSystem.Play();  // ��ƼŬ ����
            }
        }
    }
}
