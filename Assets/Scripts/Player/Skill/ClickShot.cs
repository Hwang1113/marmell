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
        if (Input.GetMouseButtonDown(0)) 
        {
            if (GetComponent<ParticleSystem>() != null)
            {
                GetComponent<ParticleSystem>().Play();
            }
        }
    }
}
