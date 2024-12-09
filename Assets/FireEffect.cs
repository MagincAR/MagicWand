using UnityEngine;

public class FireEffect : MonoBehaviour
{
    private ParticleSystem fireParticleSystem;

    void Start()
    {
        // Fireball�� ������ �� ParticleSystem�� ã���ϴ�.
        fireParticleSystem = GetComponent<ParticleSystem>();

        if (fireParticleSystem == null)
        {
            Debug.LogError("FireEffect requires a ParticleSystem on the same GameObject.");
        }
    }

    // ��Ÿ�� ȿ���� �����ϴ� �޼���
    public void StartFire()
    {
        if (fireParticleSystem != null)
        {
            fireParticleSystem.Play(); // ParticleSystem ����
        }
    }

    // ��Ÿ�� ȿ���� ���ߴ� �޼��� (�ʿ� �� �߰�)
    public void StopFire()
    {
        if (fireParticleSystem != null)
        {
            fireParticleSystem.Stop(); // ParticleSystem ����
        }
    }
}