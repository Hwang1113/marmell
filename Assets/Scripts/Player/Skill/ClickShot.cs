using UnityEngine;

public class ClickShot : MonoBehaviour
{
    public ParticleSystem chocoParticleSystem;
    void Start()
    {
        chocoParticleSystem = GetComponent<ParticleSystem>();
    }

        void Update()
    {
        // ���콺 ���� ��ư Ŭ�� �� ��ƼŬ ����
        if (Input.GetMouseButtonDown(0))  // 0�� ��Ŭ���� �ǹ�
        {
            if (GetComponent<ParticleSystem>() != null)
            {
                GetComponent<ParticleSystem>().Play();  // ��ƼŬ ����
            }
        }
    }
}
